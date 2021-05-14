using System;

namespace Dji.Network.Packet
{
    public class DjiNetworkPacket : NetworkPacket
    {
        private readonly DjiPacket _djiPacket;

        public DjiNetworkPacket(NetworkPacket networkPacket, DjiPacket djiPacket) : base(networkPacket) => _djiPacket = djiPacket;

        public DjiPacket DjiPacket => _djiPacket;
    }

    public class DjiNetworkPacket<T> : DjiNetworkPacket where T : DjiPacket
    {
        public DjiNetworkPacket(NetworkPacket networkPacket) : base(networkPacket, (T)Activator.CreateInstance(typeof(T), new object[] { networkPacket.UdpPacket })) { }

        public new T DjiPacket => (T)base.DjiPacket;
    }
}