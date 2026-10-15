using System.Runtime.CompilerServices;

namespace LSIC.App
{
    /// <summary>
    /// 报文帧过滤辅助类，根据功能码过滤 Debug 日志显示
    /// </summary>
    public static class FrameFilterUtility
    {
        private static readonly ConditionalWeakTable<FlowLogger, Type> _loggerMaps = new ConditionalWeakTable<FlowLogger, Type>();

        private static readonly ConcurrentDictionary<Type, List<FrameFilterItem>> _filterMaps = new ConcurrentDictionary<Type, List<FrameFilterItem>>();

        /// <summary>
        /// 注册报文帧过滤器
        /// </summary>
        public static void Register<T>(FlowLogger logger, Func<T, bool> defaultSelectedFunc = null) where T : Enum
        {
            // 创建过滤器集合
            _loggerMaps.Add(logger, typeof(T));

            // 创建过滤器项，如果已经存在则不重复创建
            if (_filterMaps.ContainsKey(typeof(T)))
            {
                return;
            }

            List<FrameFilterItem> filterItems = new List<FrameFilterItem>();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                FrameFilterItem item = new FrameFilterItem(value);
                if (defaultSelectedFunc is not null)
                {
                    item.IsSelected = defaultSelectedFunc(value);
                }
                filterItems.Add(item);
            }
            _filterMaps.TryAdd(typeof(T), filterItems);
        }

        /// <summary>
        /// 过滤方法
        /// </summary>
        public static bool Filter(FlowLogger logger, Enum functionCode)
        {
            if (!_loggerMaps.TryGetValue(logger, out Type enumType) || enumType is null)
            {
                return false;
            }
            if (!_filterMaps.TryGetValue(enumType, out List<FrameFilterItem> filterItems))
            {
                return false;
            }
            return filterItems.FirstOrDefault(item => item.FunctionCode.Equals(functionCode))?.IsSelected ?? false;
        }

        /// <summary>
        /// 获取过滤器项集合
        /// </summary>
        public static List<FrameFilterItem> GetFilterItems(FlowLogger logger)
        {
            if (!_loggerMaps.TryGetValue(logger, out Type enumType) || enumType is null)
            {
                return new List<FrameFilterItem>();
            }
            if (!_filterMaps.TryGetValue(enumType, out List<FrameFilterItem> filterItems))
            {
                return new List<FrameFilterItem>();
            }
            return filterItems;
        }
    }
}
