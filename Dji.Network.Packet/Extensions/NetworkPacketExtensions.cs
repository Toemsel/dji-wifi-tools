using Dji.Network.Packet.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Dji.Network.Packet.Extensions
{
    public static class NetworkPacketExtensions
    {
        public static IEnumerable<DjiNetworkPacket> ToDjiNetworkPackets(this NetworkPacket networkPacket)
        {
            byte[] networkPacketData = networkPacket.UdpPacket.Payload;

            // if we discover more than one packet, they do share the same wifi-header.
            DjiWifiHeader djiWifiHeader = null;

            for (int idx = 0; idx < networkPacketData.Length; idx++)
            {
                // a valid packet starts with 0x55
                if (networkPacketData[idx] != 0x55) continue;

                // try to create a valid header from the current index
                DjiHeader djiHeader = DjiFactory.ConvertToDjiHeader(networkPacketData, idx);

                // if we weren't able to create a valid header, continue
                if (djiHeader == null) continue;

                // try to create a valid payload from the current index
                DjiPayload djiPayload = DjiFactory.ConvertToDjiPayload(djiHeader, networkPacketData, idx);

                // if we weren't able to create a valid payload, continue
                if (djiPayload == null) continue;

                // create or re-use the existing wifi-header
                djiWifiHeader ??= DjiFactory.ConvertToWifiHeader(networkPacketData, idx);

                // cap the current 'networkPacketData' by the found information
                networkPacketData = networkPacketData.Skip(idx + djiHeader.Size).ToArray();

                // return the found packet and continue to search through
                yield return new DjiNetworkPacket(networkPacket, new DjiPacket(networkPacket.UdpPacket, djiWifiHeader, djiHeader, djiPayload));
            }

            if (networkPacketData.Length > 0)
            {
                // create or re-use the existing wifi-header
                djiWifiHeader ??= DjiFactory.ConvertToWifiHeader(networkPacketData, networkPacketData.Length);
                // return an unknown packet if we still have data left to process
                yield return new DjiNetworkPacket(networkPacket, new DjiPacket(networkPacket.UdpPacket, djiWifiHeader, null, null));
            }                
        }
    }
}