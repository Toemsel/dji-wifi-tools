namespace Dji.Network.Packet.Structure
{
    public class DjiHeader : IDjiData
    {
        private byte[] _data;

        internal DjiHeader(byte delimiter, ushort size, byte version, byte crc) =>
            (Delimiter, Size, Version, CRC) = (delimiter, size, version, crc);

        public byte Delimiter { get; init; }

        public ushort Size { get; init; }

        public byte Version { get; init; }

        public byte CRC { get; init; }

        public byte[] GetBytes() => _data ??= DjiFactory.ConvertToBytes(this);
    }
}