using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dji.Network.Packet.Extensions
{
    public static class ByteExtensions
    {
        public static IEnumerable<int> IndexOf(this byte[] data, byte value)
        {
            for (int idx = 0; idx < data.Length; idx++)
                if (data[idx] == value)
                    yield return idx;
        }

        public static bool IndexOf(this byte[] data, out int index, params byte[] value)
        {
            int searchIndex = 0;
            index = -1;

            for (int idx = 0; idx < data.Length; idx++)
            {
                searchIndex = data[idx] == value[searchIndex] ? (searchIndex + 1) : 0;

                if (searchIndex == value.Length)
                {
                    index = idx - (searchIndex - 1);
                    break;
                }                    
            }

            return index != -1;
        }

        public static bool EndsWith(this byte[] data, params byte[] value)
        {
            for (int dataIdx = data.Length - 1, valueIdx = value.Length - 1; valueIdx >= 0; dataIdx--, valueIdx--)
                if (data[dataIdx] != value[valueIdx])
                    return false;

            return true;
        }

        public static byte[] Append(this byte[] data, params byte[] toAppend)
        {
            byte[] nData = new byte[data.Length + toAppend.Length];
            Array.Copy(data, 0, nData, 0, data.Length);
            Array.Copy(toAppend, 0, nData, data.Length, toAppend.Length);
            return nData;
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

        public static byte[] FromHexString(this string hexString)
        {
            int GetHexVal(char hex)
            {
                int val = (int)hex;
                return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
            }

            // get rid of leading zero
            hexString = hexString.ToLower().Replace("0x", "");

            if (hexString.Length % 2 == 1)
                throw new ArgumentException("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hexString.Length >> 1];

            for (int i = 0; i < hexString.Length >> 1; ++i)
                arr[i] = (byte)((GetHexVal(hexString[i << 1]) << 4) + (GetHexVal(hexString[(i << 1) + 1])));

            return arr;
        }

        public static bool[] ToBits(this byte data) => Enumerable.Range(1, 8).Select(bitNumber => (data & (1 << bitNumber - 1)) != 0).Reverse().ToArray();
    }
}