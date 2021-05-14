using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Dji.Network.Packet
{
    public class UdpPacket : IEqualityComparer<UdpPacket>
    {
        public UdpPacket(byte[] data)
        {
            Data = data;
            Ethernet = Data[0..14];
            IP = Data[14..34];
            UDP = Data[34..42];
            Payload = Data[42..];

            SourceIpAddress = Data.Length < 29 ? string.Empty : 
                $"{(uint)Data[26]}.{(uint)Data[27]}.{(uint)Data[28]}.{(uint)Data[29]}";

            DestinationIpAddress = Data.Length < 33 ? string.Empty : 
                $"{(uint)Data[30]}.{(uint)Data[31]}.{(uint)Data[32]}.{(uint)Data[33]}";
        }

        public byte[] Data { get; init; }

        // 14 bytes
        public byte[] Ethernet { get; init; }

        // 20 bytes
        public byte[] IP { get; init; }

        // 8 bytes
        public byte[] UDP { get; init; }

        public byte[] Payload { get; init; }

        public bool HasPayload => Data.Length >= 42;

        public string SourceIpAddress { get; init; }

        public string DestinationIpAddress { get; init; }

        public bool Equals(UdpPacket x, UdpPacket y) => x.Data.SequenceEqual(y.Data);

        public int GetHashCode([DisallowNull] UdpPacket obj) => HashCode.Combine(obj.Data);
    }
}