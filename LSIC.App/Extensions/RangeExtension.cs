namespace LSIC.App
{
    public static class RangeExtension
    {
        public static string ToString(this Range range, long? value, long floorScale, string floorUnit, long ceilingScale, string ceilingUnit, string format = null)
        {
            Limit limit = value.HasValue
                ? range.TryGetValue(value.Value, out var l) ? l : null
                : range.Windows.FirstOrDefault().Value;

            return limit?.GetRangString(floorScale, floorUnit, ceilingScale, ceilingUnit, format) ?? string.Empty;
        }
    }
}
