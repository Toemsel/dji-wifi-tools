using Dji.Network.Packet.Structure;
using System;

namespace Dji.Network.Packet
{
    public class DjiPacket
    {
        internal DjiPacket(UdpPacket udpPacket, DjiWifiHeader wifiHeader, DjiHeader header, DjiPayload payload) =>
            (Raw, WifiHeader, Header, Payload) = (udpPacket, wifiHeader, header, payload);

        public UdpPacket Raw { get; init; }

        public DjiWifiHeader WifiHeader { get; init; }

        public DjiHeader Header { get; init; }

        public DjiPayload Payload { get; init; }

        public bool IsKnown => Header != null && Payload != null;

        public byte[] GetBytes(bool includeWifiHeader = true, bool includeHeader = true, bool includePayload = true)
        {
            int wifiHeaderSize = includeWifiHeader && WifiHeader != null ? WifiHeader.GetBytes().Length : 0;
            int headerSize = includeHeader && Header != null ? Header.GetBytes().Length : 0;
            int payloadSize = includePayload  && Payload != null ? Payload.GetBytes().Length : 0;

            byte[] data = new byte[wifiHeaderSize + headerSize + payloadSize];

            if (wifiHeaderSize > 0)
                Array.Copy(WifiHeader.GetBytes(), 0, data, 0, wifiHeaderSize);
            if (headerSize > 0)
                Array.Copy(Header.GetBytes(), 0, data, wifiHeaderSize, headerSize);
            if (payloadSize > 0)
                Array.Copy(Payload.GetBytes(), 0, data, wifiHeaderSize + headerSize, payloadSize);

            return data;
        }
    }
}