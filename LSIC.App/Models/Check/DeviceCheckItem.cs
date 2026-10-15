namespace LSIC.App.Models
{
    public class DeviceCheckItem
    {
        private DeviceCheckItem(byte type, byte item, byte result, byte location)
        {
            Type = type;
            Item = item;
            Result = result;
            Location = location;
        }

        /// <summary>
        /// 板卡类型码
        /// </summary>
        public byte Type { get; }

        /// <summary>
        /// 检测项码
        /// </summary>
        public byte Item { get; }

        /// <summary>
        /// 检测结果码（0x00 表示通过）
        /// </summary>
        public byte Result { get; }

        /// <summary>
        /// 故障位置（仅结果非通过时有意义）
        /// </summary>
        public byte Location { get; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool Passed => Result == 0x00;

        /// <summary>
        /// 板卡类型描述
        /// </summary>
        public string TypeText => LangProvider.Instance.GetString($"DeviceCheck.Type[{Type:X2}]");

        /// <summary>
        /// 检测项描述
        /// </summary>
        public string ItemText => LangProvider.Instance.GetString($"DeviceCheck.Item[{Item:X2}]");

        /// <summary>
        /// 检测结果描述
        /// </summary>
        public string ResultText => LangProvider.Instance.GetString($"DeviceCheck.Result[{Result:X2}]");

        /// <summary>
        /// 展示文本：板卡类型 检测项 > 结果；失败时附带位置
        /// </summary>
        public string Text
        {
            get
            {
                string text = $"{TypeText}  {ItemText}  >  {ResultText}";
                return Passed ? text : $"{text}  [{Location:D2}]";
            }
        }

        /// <summary>
        /// 从4字节解析自检结果。
        /// </summary>
        public static DeviceCheckItem FromBytes(byte[] bytes)
        {
            if (bytes is null || bytes.Length != 4)
            {
                throw new ArgumentException("DeviceCheck payload length must be 5.", nameof(bytes));
            }

            return new DeviceCheckItem(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
        public override string ToString()
        {
            return Text;
        }
    }
}
