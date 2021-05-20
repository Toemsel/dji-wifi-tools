using Dji.Network.Packet.DjiPackets.Base;
using SharpPcap;
using System.Threading;

namespace Dji.Network.Packet
{
    public class NetworkPacket
    {
        private readonly int _id;
        private readonly RawCapture _rawCapture;
        private readonly string _unixTime;
        private readonly Participant _participant;
        private readonly string _source;
        private readonly string _destination;

        private static int network_packet_counter;

        public NetworkPacket(Participant participant, string source, string destination, RawCapture rawCapture)
        {
            _id = Interlocked.Increment(ref network_packet_counter);

            _source = source;
            _destination = destination;

            _rawCapture = rawCapture;
            _participant = participant;

            _unixTime = $"{_rawCapture.Timeval.Seconds}." +
                $"{_rawCapture.Timeval.MicroSeconds}";
        }

        public NetworkPacket(NetworkPacket networkPacket)
        {
            _id = networkPacket.Id;
            _source = networkPacket.Source;
            _destination = networkPacket.Destination;
            _rawCapture = networkPacket.RawCapture;
            _unixTime = networkPacket.UnixTime;
            _participant = networkPacket.Participant;
        }

        public int Id => _id;

        public string Source => _source;

        public string Destination => _destination;

        public RawCapture RawCapture => _rawCapture;

        public Participant Participant => _participant;

        public string UnixTime => _unixTime;

        public byte[] Payload => RawCapture.Data[42..];

        public int Length => Payload.Length + 42;
    }

    public class DjiNetworkPacket : NetworkPacket
    {
        private readonly DjiPacket _djiPacket;

        public DjiNetworkPacket(NetworkPacket networkPacket, DjiPacket djiPacket) : base(networkPacket) => _djiPacket = djiPacket;

        public DjiPacket DjiPacket => _djiPacket;
    }

    public class DjiNetworkPacket<T> : DjiNetworkPacket where T : DjiPacket
    {
        public DjiNetworkPacket(NetworkPacket networkPacket, T djiPacket) : base(networkPacket, djiPacket) { }

        public new T DjiPacket => (T)base.DjiPacket;
    }
}