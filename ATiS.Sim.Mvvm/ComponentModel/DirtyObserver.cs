using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace ATiS.Sim.Mvvm
{
    public interface IManualDirtyNotifier
    {
        event EventHandler? ManualDirtyRequested;
    }

    public sealed class DirtyObserver<T> : ObservableObject, IDisposable where T : class
    {
        private bool _disposed;
        private readonly HashSet<INotifyPropertyChanged> _trackedInpc = new();
        private readonly HashSet<INotifyCollectionChanged> _trackedInc = new();
        private readonly HashSet<IManualDirtyNotifier> _trackedManual = new();
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public T? Target { get; private set; }
        public event EventHandler? IsDirtyChanged;
        public DirtyObserver(T rootTarget)
        {
            ArgumentNullException.ThrowIfNull(rootTarget);
            SubscribeRecursive(rootTarget, new HashSet<object>());
            Target = rootTarget;
        }
        public void MarkDirty()
        {
            SetDirty();
        }
        public void Reset()
        {
            if (_disposed)
            {
                return;
            }
            IsDirty = false;
        }
        private void SetDirty()
        {
            if (_disposed)
            {
                return;
            }
            IsDirty = true;
        }
        private void SubscribeRecursive(object? obj, HashSet<object> visited)
        {
            if (_disposed || obj == null)
            {
                return;
            }

            if (!visited.Add(obj))
            {
                return;
            }

            if (obj is INotifyPropertyChanged inpc)
            {
                if (_trackedInpc.Add(inpc))
                {
                    PropertyChangedEventManager.AddHandler(inpc, OnAnyPropertyChanged, string.Empty);
                }
            }

            if (obj is INotifyCollectionChanged inc && obj is not string)
            {
                if (_trackedInc.Add(inc))
                {
                    CollectionChangedEventManager.AddHandler(inc, OnAnyCollectionChanged);
                }
            }

            if (obj is IManualDirtyNotifier manual)
            {
                if (_trackedManual.Add(manual))
                {
                    manual.ManualDirtyRequested += OnManualDirtyRequested;
                }
            }

            if (obj is IEnumerable enumerable && obj is not string)
            {
                foreach (var item in enumerable)
                {
                    SubscribeRecursive(item, visited);
                }
            }

            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                if (prop.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (!prop.CanRead)
                {
                    continue;
                }

                object? value;
                try
                {
                    value = prop.GetValue(obj);
                }
                catch
                {
                    // 异常属性直接跳过
                    continue;
                }

                SubscribeRecursive(value, visited);
            }
        }
        private void UnsubscribeRecursive(object? obj)
        {
            if (_disposed || obj == null)
            {
                return;
            }

            if (obj is INotifyPropertyChanged inpc && _trackedInpc.Remove(inpc))
            {
                PropertyChangedEventManager.RemoveHandler(inpc, OnAnyPropertyChanged, string.Empty);
            }

            if (obj is INotifyCollectionChanged inc && _trackedInc.Remove(inc))
            {
                CollectionChangedEventManager.RemoveHandler(inc, OnAnyCollectionChanged);
            }

            if (obj is IManualDirtyNotifier manual && _trackedManual.Remove(manual))
            {
                manual.ManualDirtyRequested -= OnManualDirtyRequested;
            }

            if (obj is IEnumerable enumerable && obj is not string)
            {
                foreach (var item in enumerable)
                {
                    UnsubscribeRecursive(item);
                }
            }
        }
        private void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            SetDirty();

            // 反射取出该属性的新值并递归订阅
            if (sender != null && !string.IsNullOrEmpty(e.PropertyName))
            {
                try
                {
                    PropertyInfo? prop = sender.GetType().GetProperty(e.PropertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.GetIndexParameters().Length == 0 && prop.CanRead)
                    {
                        object? newValue = prop.GetValue(sender);
                        if (newValue != null)
                        {
                            SubscribeRecursive(newValue, new HashSet<object>());
                        }
                    }
                }
                catch
                {
                }
            }
        }
        private void OnAnyCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            SetDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        HashSet<object> visited = new();
                        foreach (var item in e.NewItems)
                        {
                            SubscribeRecursive(item, visited);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            UnsubscribeRecursive(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            UnsubscribeRecursive(item);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        HashSet<object> visited = new();
                        foreach (var item in e.NewItems)
                        {
                            SubscribeRecursive(item, visited);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (sender is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            UnsubscribeRecursive(item);
                        }
                        HashSet<object> visited = new();
                        foreach (var item in enumerable)
                        {
                            SubscribeRecursive(item, visited);
                        }
                    }
                    break;
            }
        }
        private void OnManualDirtyRequested(object? sender, EventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            SetDirty();
        }
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            foreach (var inpc in _trackedInpc)
            {
                PropertyChangedEventManager.RemoveHandler(inpc, OnAnyPropertyChanged, string.Empty);
            }
            _trackedInpc.Clear();

            foreach (var inc in _trackedInc)
            {
                CollectionChangedEventManager.RemoveHandler(inc, OnAnyCollectionChanged);
            }
            _trackedInc.Clear();

            foreach (var manual in _trackedManual)
            {
                manual.ManualDirtyRequested -= OnManualDirtyRequested;
            }
            _trackedManual.Clear();

            IsDirtyChanged = null;
            Target = null;

        }
    }
}