namespace Dji.Network.Packet.Structure
{
    public class DjiWifiHeader : IDjiData
    {
        private byte[] _data;

        internal DjiWifiHeader(byte size, byte protocol, byte[] session, byte[] payload) =>
            (Size, Protocol, Session, Payload) = (size, protocol, session, payload);

        public byte Size { get; init; }

        public byte Protocol { get; init; }

        public byte[] Session { get; init; }

        public byte[] Payload { get; init; }

        public byte[] GetBytes() => _data ??= DjiFactory.ConvertToBytes(this);
    }
}