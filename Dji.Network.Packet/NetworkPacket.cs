using System.Threading;
using SharpPcap;

namespace Dji.Network.Packet
{
    public class NetworkPacket
    {
        private readonly int _id;
        private readonly UdpPacket _udpPacket;
        private readonly RawCapture _rawCapture;
        private readonly string _unixTime;

        private static int network_packet_counter;

        public NetworkPacket(UdpPacket udpPacket, RawCapture rawCapture)
        {
            _id = Interlocked.Increment(ref network_packet_counter);

            _udpPacket = udpPacket;
            _rawCapture = rawCapture;

            _unixTime = $"{_rawCapture.Timeval.Seconds}." +
                $"{_rawCapture.Timeval.MicroSeconds}";
        }

        public NetworkPacket(NetworkPacket networkPacket)
        {
            _id = networkPacket.Id;
            _udpPacket = networkPacket.UdpPacket;
            _rawCapture = networkPacket.RawCapture;
            _unixTime = networkPacket.UnixTime;
        }

        public int Id => _id;

        public UdpPacket UdpPacket => _udpPacket;

        public RawCapture RawCapture => _rawCapture;

        public string UnixTime => _unixTime;
    }
}
