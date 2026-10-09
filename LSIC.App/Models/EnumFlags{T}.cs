using System.Runtime.CompilerServices;

namespace LSIC.App.Models
{
    public class EnumFlags<T> : ObservableObject where T : Enum
    {
        public EnumFlags(T t)
        {
            Value = t;
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged("Item[]");
                }
            }
        }

        [IndexerName("Item")]
        public bool this[T key]
        {
            get
            {
                ulong v = Convert.ToUInt64(_value);
                ulong k = Convert.ToUInt64(key);
                return (v & k) == k;
            }
            set
            {
                ulong v = Convert.ToUInt64(_value);
                ulong k = Convert.ToUInt64(key);

                bool currentlySet = (v & k) == k;
                if (currentlySet == value)
                {
                    return;
                }

                ulong newV = value ? (v | k) : (v & ~k);
                T newEnum = (T)Enum.ToObject(typeof(T), newV);

                // 使用 SetProperty 确保通知到 Value 的变更
                if (SetProperty(ref _value, newEnum, nameof(Value)))
                {
                    // 通知索引器绑定刷新
                    OnPropertyChanged("Item[]");
                }
            }
        }

        public bool HasFlag(T flag)
        {
            ulong v = Convert.ToUInt64(_value);
            ulong k = Convert.ToUInt64(flag);
            return (v & k) == k;
        }
    }
}
