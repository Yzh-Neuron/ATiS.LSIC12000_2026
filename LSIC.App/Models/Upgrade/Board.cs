namespace LSIC.App.Models
{
    public class Board : ObservableObject
    {
        public Board(BoardTypeInfo type, byte id, byte parentId, string pn, string sn, char hardwareVersion, FirmwareInfo[] firmwares)
        {
            Type = type;
            Id = id;
            ParentId = parentId;
            PN = pn;
            SN = sn;
            HardwareVersion = hardwareVersion;
            Firmwares = firmwares;
        }
        public BoardTypeInfo Type { get; }
        public byte Id { get; }
        public byte ParentId { get; }
        public bool HasParent => ParentId != 0xFF;
        public byte TypeCode => (byte)Type.TypeCode;
        public string DisplayName => LangProvider.Instance.GetString(Type.BoardKey);
        public bool Upgradable => Type.Upgradable;
        public string PN { get; }
        public string SN { get; }
        public char HardwareVersion { get; }
        public FirmwareInfo[] Firmwares { get; }
        public static Board FromBytes(byte[] data, IReadOnlyDictionary<byte, BoardTypeInfo> types)
        {
            int index = 0;
            byte code = data[index++];
            byte id = data[index++];
            byte parent = data[index++];

            string pn = Encoding.ASCII.GetString(data.Skip(index).Take(16).SkipWhile(i => i == 0xFF).TakeWhile(i => i != 0).ToArray());
            index += 16;
            string sn = Encoding.ASCII.GetString(data.Skip(index).Take(16).SkipWhile(i => i == 0xFF).TakeWhile(i => i != 0).ToArray());
            index += 16;

            byte hardware = (byte)(data[index++] & 0x0F);
            char hw = hardware == 0 ? '0' : (char)(hardware - 1 + 'A');

            int firmwareCount = data[index++];
            FirmwareInfo[] firmwares = new FirmwareInfo[firmwareCount];
            for (int k = 0; k < firmwareCount; k++)
            {
                FirmwareType ft = (FirmwareType)data[index++];
                int version = data.ToInt32(index, Endianness.Big);
                index += 4;
                long compile = data.ToInt64(index, Endianness.Big);
                index += 8;
                long upgrade = data.ToInt64(index, Endianness.Big);
                index += 8;
                firmwares[k] = new FirmwareInfo(
                    ft, version,
                    compile >= 0 ? compile.Time() : null,
                    upgrade >= 0 ? upgrade.Time() : null);
            }

            return new Board(types.TryGetValue(code, out var def) ? def : BoardTypeInfo.Unknown(code), id, parent, pn, sn, hw, firmwares);
        }
    }
}