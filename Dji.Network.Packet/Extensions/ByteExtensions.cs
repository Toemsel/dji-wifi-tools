using NullFX.CRC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dji.Network.Packet.Extensions
{
    public static class ByteExtensions
    {
        public static byte[] Crc16_Kermit(this byte[] data) => BitConverter.GetBytes(Crc16.ComputeChecksum(Crc16Algorithm.CcittKermit, data));

        public static byte Crc8(this byte data) => Crc8(new byte[] { data });

        public static byte Crc8(this byte[] data) => NullFX.CRC.Crc8.ComputeChecksum(data);

        public static IEnumerable<int> IndexOf(this byte[] data, byte value)
        {
            for (int idx = 0; idx < data.Length; idx++)
                if (data[idx] == value)
                    yield return idx;
        }

        public static bool IsEqual(this byte data, string hex) => IsEqual(new byte[] { data }, hex);

        public static bool IsEqual(this byte[] data, string hex) => data.ToHexString(true, true).ToLower() == hex.ToLower().Trim();

        public static bool Contains(this byte data, string hex) => Contains(new byte[] { data }, hex);

        public static bool Contains(this byte[] data, string hex) => data.ToHexString(true, true).ToLower().Contains(hex.ToLower().Trim());

        public static string ToHexString(this byte data, bool useLeadingZero = true) => ToHexString(new byte[] { data }, useLeadingZero);

        public static string ToHexString(this byte[] data, bool useLeadingZero = true, bool useSpacing = true)
        {
            var hex = new StringBuilder(data.Length * 2);

            for(int index = 0; index < data.Length; index++)
            {
                if (useLeadingZero) hex.Append("0x");
                hex.AppendFormat("{0:x2}", data[index]);
                if (useSpacing && index < data.Length - 1) hex.Append(' ');
            }
            
            return hex.ToString();
        }

        public static bool[] ToBits(this byte data) => Enumerable.Range(1, 8).Select(bitNumber => (data & (1 << bitNumber - 1)) != 0).Reverse().ToArray();
    }
}