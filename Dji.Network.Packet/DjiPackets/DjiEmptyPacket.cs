using Dji.Network.Packet.DjiPackets.Base;

namespace Dji.Network.Packet.DjiPackets
{
    public class DjiEmptyPacket : DjiPacket
    {
        public DjiEmptyPacket() { }

        protected override byte[] Build() => new byte[] { };

        protected override bool Build(byte[] data) => true;
    }
}
