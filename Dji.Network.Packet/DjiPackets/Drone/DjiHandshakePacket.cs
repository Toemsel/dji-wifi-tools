using Dji.Network.Packet.DjiPackets.Base;

namespace Dji.Network.Packet.DjiPackets.Drone
{
    public class DjiHandshakePacket : DjiPacket
    {
        private byte _crc;

        public byte Crc
        {
            get => _crc;
            set => SetValue(ref _crc, value);
        }

        protected override byte[] Build()
        {
            return new byte[4]
            {
                0x00,
                0x00,
                0x00,
                Crc
            };
        }

        protected override bool Build(byte[] data)
        {
            if (data.Length != 4) return false;
            else if (data[0] != 0x00 || data[1] != 0x00 || data[2] != 0x00) return false;
            else if (!DjiCrc.Crc8(Session[0], Session[1], data[3])) return false;

            Crc = data[3];
            return true;
        }
    }
}
