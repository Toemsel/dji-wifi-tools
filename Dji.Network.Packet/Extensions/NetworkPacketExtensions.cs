using Dji.Network.Packet.DjiPackets.Base;
using System;

namespace Dji.Network.Packet.Extensions
{
    public static class NetworkPacketExtensions
    {
        public static DjiNetworkPacket Wrap(this NetworkPacket networkPacket, DjiPacket djiPacket) => new DjiNetworkPacket(networkPacket, djiPacket);

        public static dynamic Wrap(this NetworkPacket networkPacket, DjiPacket djiPacket, Type packetType)
        {
            var djiNetworkPacketType = typeof(DjiNetworkPacket<>).MakeGenericType(packetType);
            return Activator.CreateInstance(djiNetworkPacketType, new object[] { networkPacket, djiPacket });
        }
    }
}