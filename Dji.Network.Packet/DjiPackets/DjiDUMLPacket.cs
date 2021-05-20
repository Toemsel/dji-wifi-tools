using Dji.Network.Packet.DjiPackets.Base;
using System;

namespace Dji.Network.Packet.DjiPackets
{
    public abstract class DjiDUMLPacket : DjiPacket
    {
        protected const ushort HEADER_SIZE = 4;
        protected const ushort PAYLOAD_SIZE = 9;

        private byte _delimiter;
        private ushort _dumlSize;
        private byte _version;
        private byte _crc;

        public byte Delimiter
        {
            get => _delimiter;
            set => SetValue(ref _delimiter, value);
        }

        public ushort DumlSize
        {
            get => _dumlSize;
            set => SetValue(ref _dumlSize, value);
        }

        public byte Version
        {
            get => _version;
            set => SetValue(ref _version, value);
        }

        public byte DUMLCrc
        {
            get => _crc;
            set => SetValue(ref _crc, value);
        }

        protected override byte[] Build()
        {
            ushort version = (ushort)(Version << 10);
            ushort sum = (ushort)(version + DumlSize);

            return new byte[]
            {
                Delimiter,
                (byte)sum,
                (byte)(sum >> 8),
                DUMLCrc,
            };
        }

        protected override bool Build(byte[] data)
        {
            // the header doesn't fit into the provided byte array
            if (data.Length < HEADER_SIZE) return false;

            byte delimiter = data[0];
            byte version = (byte)((data[2] & 0xFC) >> 2);
            ushort size = (ushort)(BitConverter.ToUInt16(data[1..3], 0) & 0x3FF);
            byte crc = data[3];

            // unknown delimiter received
            if (delimiter != 0x55) return false;
            // crc isn't valid
            else if (!DjiCrc.Crc8(data[..3], data[3])) return false;
            // the size isn't valid - hence, no space for a payload
            else if (size < Math.Max((ushort)(HEADER_SIZE + PAYLOAD_SIZE), size)) return false;

            // as all parameters are valid, we can set the object values
            Delimiter = delimiter;
            Version = version;
            DumlSize = size;
            DUMLCrc = crc;
            return true;
        }
    }
}
