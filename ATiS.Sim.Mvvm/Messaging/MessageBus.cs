using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ATiS.Sim.Mvvm
{
    public sealed class MessageBus
    {
        public static MessageBus Default { get; } = new();
        private readonly object _lock = new();
        private static readonly object Sentinel = new();
        private readonly Dictionary<TypePair, ConditionalWeakTable<object, object>> _map = new();

        // 订阅注册
        public void Register<TMessage>(IRecipient<TMessage> recipient)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            RegisterInternal<TMessage, Unit>(recipient, Unit.Value);
        }
        public void Register<TMessage, TToken>(IRecipient<TMessage> recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(token);
            RegisterInternal<TMessage, TToken>(recipient, token);
        }
        public void Register<TMessage>(IRecipientAsync<TMessage> recipient)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            RegisterInternal<TMessage, Unit>(recipient, Unit.Value);
        }
        public void Register<TMessage, TToken>(IRecipientAsync<TMessage> recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(token);
            RegisterInternal<TMessage, TToken>(recipient, token);
        }
        private void RegisterInternal<TMessage, TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            EnsureNotDualImplementation<TMessage>(recipient);

            lock (_lock)
            {
                TypePair key = new(typeof(TMessage), typeof(TToken));

                if (!_map.TryGetValue(key, out var cwt))
                {
                    cwt = new ConditionalWeakTable<object, object>();
                    _map.Add(key, cwt);
                }

                if (typeof(TToken) == typeof(Unit))
                {
                    if (!cwt.TryAdd(recipient, Sentinel))
                    {
                        string? recipientType = recipient.GetType().FullName;
                        string? messageType = typeof(TMessage).FullName;
                        throw new InvalidOperationException($"Recipient [{recipientType}] has already subscribed to message [{messageType}] (default channel).");
                    }
                }
                else
                {
                    if (!cwt.TryGetValue(recipient, out var tokenSetObj))
                    {
                        tokenSetObj = new HashSet<TToken>();
                        cwt.Add(recipient, tokenSetObj);
                    }

                    var tokenSet = (HashSet<TToken>)tokenSetObj;
                    if (!tokenSet.Add(token))
                    {
                        string? recipientType = recipient.GetType().FullName;
                        string? messageType = typeof(TMessage).FullName;
                        string? tokenType = typeof(TToken).FullName;
                        throw new InvalidOperationException($"Recipient [{recipientType}] has already subscribed to message [{messageType}] with token [{tokenType}].");

                    }
                }
            }
        }

        // 消息发送
        public MessageResult[] Send<TMessage>(TMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return SendInternal(message, Unit.Value);
        }
        public MessageResult[] Send<TMessage, TToken>(TMessage message, TToken token)
            where TToken : IEquatable<TToken>
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(token);
            return SendInternal(message, token);
        }
        public async ValueTask<MessageResult[]> SendAsync<TMessage>(TMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return await SendInternalAsync(message, Unit.Value);
        }
        public async ValueTask<MessageResult[]> SendAsync<TMessage, TToken>(TMessage message, TToken token)
            where TToken : IEquatable<TToken>
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(token);
            return await SendInternalAsync(message, token);
        }
        private MessageResult[] SendInternal<TMessage, TToken>(TMessage message, TToken token)
            where TToken : IEquatable<TToken>
        {
            List<object> snapshot = GetSnapshot<TMessage, TToken>(token);
            List<MessageResult> resultList = new();

            foreach (var recipient in snapshot)
            {
                if (recipient is IRecipientAsync<TMessage> asyncRecipient)
                {
                    var recType = recipient.GetType().FullName;
                    var msgType = typeof(TMessage).FullName;
                    throw new InvalidOperationException($"MessageBus.Send: Recipient [{recType}] is IRecipientAsync<{msgType}>. Please use SendAsync instead of Send.");
                }
                else if (recipient is IRecipient<TMessage> syncRecipient)
                {
                    try
                    {
                        var wrap = syncRecipient.Receive(message);
                        if (wrap != null)
                        {
                            wrap.RecipientType = syncRecipient.GetType();
                            resultList.Add(wrap);
                        }
                    }
                    catch
                    {

                    }
                }

            }

            return resultList.ToArray();
        }
        private async Task<MessageResult[]> SendInternalAsync<TMessage, TToken>(TMessage message, TToken token)
           where TToken : IEquatable<TToken>
        {
            List<object> snapshot = GetSnapshot<TMessage, TToken>(token);
            List<MessageResult> resultList = new();

            foreach (var recipient in snapshot)
            {
                try
                {
                    if (recipient is IRecipientAsync<TMessage> asyncRecipient)
                    {
                        var wrap = await asyncRecipient.ReceiveAsync(message);
                        if (wrap != null)
                        {
                            wrap.RecipientType = asyncRecipient.GetType();
                            resultList.Add(wrap);
                        }
                    }
                    else if (recipient is IRecipient<TMessage> syncRecipient)
                    {
                        var wrap = syncRecipient.Receive(message);
                        if (wrap != null)
                        {
                            wrap.RecipientType = syncRecipient.GetType();
                            resultList.Add(wrap);
                        }
                    }
                }
                catch
                {

                }
            }

            return resultList.ToArray();
        }
        private List<object> GetSnapshot<TMessage, TToken>(TToken token)
            where TToken : IEquatable<TToken>
        {
            List<object> snapshot = new();
            lock (_lock)
            {
                TypePair key = new(typeof(TMessage), typeof(TToken));
                if (!_map.TryGetValue(key, out var cwt))
                {
                    return snapshot;
                }

                foreach (var kv in cwt)
                {
                    object recipient = kv.Key;
                    if (typeof(TToken) == typeof(Unit))
                    {
                        if (ReferenceEquals(kv.Value, Sentinel))
                        {
                            snapshot.Add(recipient);
                        }
                    }
                    else
                    {
                        if (kv.Value is HashSet<TToken> tokenSet && tokenSet.Contains(token))
                        {
                            snapshot.Add(recipient);
                        }
                    }
                }

                CleanupEmptyTables();
            }
            return snapshot;
        }

        // 取消订阅
        public void UnregisterAll(object recipient)
        {
            ArgumentNullException.ThrowIfNull(recipient);

            lock (_lock)
            {
                foreach (var kv in _map)
                {
                    kv.Value.Remove(recipient);
                }
                CleanupEmptyTables();
            }
        }
        public void Unregister<TMessage>(object recipient)
        {
            UnregisterInternal<TMessage, Unit>(recipient, Unit.Value);
        }
        public void Unregister<TMessage, TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            UnregisterInternal<TMessage, TToken>(recipient, token);
        }
        private void UnregisterInternal<TMessage, TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            ArgumentNullException.ThrowIfNull(recipient);

            lock (_lock)
            {
                TypePair key = new(typeof(TMessage), typeof(TToken));
                if (!_map.TryGetValue(key, out var cwt))
                {
                    return;
                }

                if (typeof(TToken) == typeof(Unit))
                {
                    cwt.Remove(recipient);
                }
                else
                {
                    if (cwt.TryGetValue(recipient, out var tokenSetObj)
                        && tokenSetObj is HashSet<TToken> tokenSet)
                    {
                        tokenSet.Remove(token);
                        if (tokenSet.Count == 0)
                        {
                            cwt.Remove(recipient);
                        }
                    }
                }

                CleanupEmptyTables();
            }
        }

        //辅助方法
        public bool IsRegistered<TMessage, TToken>(object recipient, TToken token)
            where TToken : IEquatable<TToken>
        {
            ArgumentNullException.ThrowIfNull(recipient);

            lock (_lock)
            {
                TypePair key = new(typeof(TMessage), typeof(TToken));
                if (!_map.TryGetValue(key, out var cwt))
                {
                    return false;
                }

                if (typeof(TToken) == typeof(Unit))
                {
                    return cwt.TryGetValue(recipient, out _);
                }

                return cwt.TryGetValue(recipient, out var tokenSetObj)
                       && tokenSetObj is HashSet<TToken> tokenSet
                       && tokenSet.Contains(token);
            }
        }
        public void Cleanup()
        {
            lock (_lock)
            {
                CleanupEmptyTables();
            }
        }
        public void Reset()
        {
            lock (_lock)
            {
                _map.Clear();
            }
        }
        private void CleanupEmptyTables()
        {
            List<TypePair> toRemove = new();
            foreach (var kv in _map)
            {
                bool hasItems = false;
                foreach (var _ in kv.Value)
                {
                    hasItems = true;
                    break;
                }
                if (!hasItems)
                {
                    toRemove.Add(kv.Key);
                }
            }
            foreach (var key in toRemove)
            {
                _map.Remove(key);
            }
        }
        private void EnsureNotDualImplementation<TMessage>(object recipient)
        {
            bool isSync = recipient is IRecipient<TMessage>;
            bool isAsync = recipient is IRecipientAsync<TMessage>;

            if (isSync && isAsync)
            {
                string? recType = recipient.GetType().FullName;
                string? msgType = typeof(TMessage).FullName;
                throw new InvalidOperationException(
                    $"Recipient [{recType}] must implement either IRecipient<{msgType}> OR IRecipientAsync<{msgType}>, not both. Do not implement two interfaces for the same message type.");
            }
        }
    }
}