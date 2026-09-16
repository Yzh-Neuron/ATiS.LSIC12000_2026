using System;
using System.Threading.Tasks;

namespace ATiS.Sim.Mvvm
{
    public interface IRecipient<in TMessage>
    {
        MessageResult? Receive(TMessage e);
    }

    public interface IRecipientAsync<in TMessage>
    {
        ValueTask<MessageResult?> ReceiveAsync(TMessage e);
    }

    internal readonly struct Unit : IEquatable<Unit>
    {
        internal static readonly Unit Value = default;

        public bool Equals(Unit other) => true;
        public override bool Equals(object? obj) => obj is Unit;
        public override int GetHashCode() => 0;
    }

    internal readonly struct TypePair : IEquatable<TypePair>
    {
        internal Type MessageType { get; }
        internal Type TokenType { get; }

        public TypePair(Type messageType, Type tokenType)
        {
            MessageType = messageType;
            TokenType = tokenType;
        }

        public bool Equals(TypePair other)
            => MessageType == other.MessageType && TokenType == other.TokenType;

        public override bool Equals(object? obj)
            => obj is TypePair other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(MessageType, TokenType);
    }
}
