using Dji.Network.Packet.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace Dji.Network.Packet.Structure
{
    public static class DjiFactory
    {
        private const int HEADER_SIZE = 4;
        private const int PAYLOAD_SIZE = 9;

        private static readonly Dictionary<Cmd, CmdAttribute> _commandAttributes = new();

        private static readonly byte[] headerCrcHexTable = new byte[]
{
            0x00,0x5E,0xBC,0xE2,0x61,0x3F,0xDD,0x83,0xC2,0x9C,0x7E,0x20,0xA3,0xFD,0x1F,0x41,
            0x9D,0xC3,0x21,0x7F,0xFC,0xA2,0x40,0x1E,0x5F,0x01,0xE3,0xBD,0x3E,0x60,0x82,0xDC,
            0x23,0x7D,0x9F,0xC1,0x42,0x1C,0xFE,0xA0,0xE1,0xBF,0x5D,0x03,0x80,0xDE,0x3C,0x62,
            0xBE,0xE0,0x02,0x5C,0xDF,0x81,0x63,0x3D,0x7C,0x22,0xC0,0x9E,0x1D,0x43,0xA1,0xFF,
            0x46,0x18,0xFA,0xA4,0x27,0x79,0x9B,0xC5,0x84,0xDA,0x38,0x66,0xE5,0xBB,0x59,0x07,
            0xDB,0x85,0x67,0x39,0xBA,0xE4,0x06,0x58,0x19,0x47,0xA5,0xFB,0x78,0x26,0xC4,0x9A,
            0x65,0x3B,0xD9,0x87,0x04,0x5A,0xB8,0xE6,0xA7,0xF9,0x1B,0x45,0xC6,0x98,0x7A,0x24,
            0xF8,0xA6,0x44,0x1A,0x99,0xC7,0x25,0x7B,0x3A,0x64,0x86,0xD8,0x5B,0x05,0xE7,0xB9,
            0x8C,0xD2,0x30,0x6E,0xED,0xB3,0x51,0x0F,0x4E,0x10,0xF2,0xAC,0x2F,0x71,0x93,0xCD,
            0x11,0x4F,0xAD,0xF3,0x70,0x2E,0xCC,0x92,0xD3,0x8D,0x6F,0x31,0xB2,0xEC,0x0E,0x50,
            0xAF,0xF1,0x13,0x4D,0xCE,0x90,0x72,0x2C,0x6D,0x33,0xD1,0x8F,0x0C,0x52,0xB0,0xEE,
            0x32,0x6C,0x8E,0xD0,0x53,0x0D,0xEF,0xB1,0xF0,0xAE,0x4C,0x12,0x91,0xCF,0x2D,0x73,
            0xCA,0x94,0x76,0x28,0xAB,0xF5,0x17,0x49,0x08,0x56,0xB4,0xEA,0x69,0x37,0xD5,0x8B,
            0x57,0x09,0xEB,0xB5,0x36,0x68,0x8A,0xD4,0x95,0xCB,0x29,0x77,0xF4,0xAA,0x48,0x16,
            0xE9,0xB7,0x55,0x0B,0x88,0xD6,0x34,0x6A,0x2B,0x75,0x97,0xC9,0x4A,0x14,0xF6,0xA8,
            0x74,0x2A,0xC8,0x96,0x15,0x4B,0xA9,0xF7,0xB6,0xE8,0x0A,0x54,0xD7,0x89,0x6B,0x35 };

        private static readonly ushort[] bodyCrcHexTable = new ushort[]
        {
            0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
            0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
            0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
            0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
            0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
            0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
            0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
            0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
            0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
            0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
            0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
            0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
            0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
            0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
            0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
            0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
            0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
            0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
            0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
            0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
            0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
            0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
            0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
            0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
            0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
            0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
            0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
            0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
            0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
            0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
            0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
            0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
        };

        static DjiFactory()
        {
            foreach (var enumVal in Enum.GetValues(typeof(Cmd)))
            {
                var memberInfo = typeof(Cmd).GetMember(enumVal.ToString());
                var enumValMemberInfo = memberInfo.FirstOrDefault(m => m.DeclaringType == typeof(Cmd));
                var enumAttribute = enumValMemberInfo.GetCustomAttributes(typeof(CmdAttribute), false);

                if (enumAttribute == null || enumAttribute.Length <= 0)
                    Trace.TraceWarning($"Enum {typeof(Cmd).Name} misses {nameof(CmdAttribute)}");

                var attribute = enumAttribute?.FirstOrDefault() as CmdAttribute;

                if (attribute != null)
                    _commandAttributes.Add((Cmd)enumVal, attribute);
            }
        }

        private static bool ValidateHeader(byte[] header)
        {
            var checkSum = 0x77;

            for (int i = 0; i < 3; i++)
                checkSum = headerCrcHexTable[((header[i] ^ checkSum) & 0xFF)];

            return checkSum == header[3];
        }

        private static bool ValidateBody(byte[] body)
        {
            ushort crc = BitConverter.ToUInt16(new byte[] { body[^2..][0], body[^2..][1] });
            ushort checkSum = 0x3692;

            for (int i = 0; i < body.Length - 2; i++)
                checkSum = (ushort)((checkSum >> 8) ^ bodyCrcHexTable[(body[i] ^ checkSum) & 0xFF]);

            return checkSum == crc;
        }

        internal static DjiWifiHeader ConvertToWifiHeader(byte[] networkPacketData, int idx)
        {
            byte packetSize = networkPacketData[0];
            byte protocol = networkPacketData[1];
            byte[] session = networkPacketData[2..4];
            byte[] payload = networkPacketData[4..idx];

            return new DjiWifiHeader(packetSize, protocol, session, payload);
        }

        internal static DjiHeader ConvertToDjiHeader(byte[] networkPacketData, int idx)
        {
            // the header doesn't fit into the provided byte array
            if (networkPacketData[idx..].Length < HEADER_SIZE) return null;

            byte delimiter = networkPacketData[idx];
            byte version = (byte)((networkPacketData[idx + 2] & 0xFC) >> 2);
            ushort size = (ushort)(BitConverter.ToUInt16(networkPacketData[(idx + 1)..(idx + 3)], 0) & 0x3FF);
            byte crc = networkPacketData[idx + 3];

            // unknown delimiter received
            if (delimiter != 0x55) return null;
            // crc isn't valid
            else if (!ValidateHeader(networkPacketData[idx..(idx + 4)])) return null;
            // the size isn't valid - hence, no space for a payload
            else if (size < HEADER_SIZE + PAYLOAD_SIZE) return null;

            return new DjiHeader(delimiter, size, version, crc);
        }

        internal static DjiPayload ConvertToDjiPayload(DjiHeader djiHeader, byte[] networkPacketData, int idx)
        {
            // the body doesn't fit into the provided djiHeader
            if (networkPacketData[idx..].Length < djiHeader.Size) return null;

            // remove the header from the array
            int minIdx = idx + HEADER_SIZE;
            int maxIdx = idx + djiHeader.Size;

            byte senderIndex = (byte)((networkPacketData[minIdx] & 0xE0) >> 5);
            Transceiver sender = (Transceiver)(networkPacketData[minIdx] & 0x1f);
            byte receiverIndex = (byte)((networkPacketData[minIdx + 1] & 0xE0) >> 5);
            Transceiver receiver = (Transceiver)(networkPacketData[minIdx + 1] & 0x1f);
            ushort counter = BitConverter.ToUInt16(networkPacketData[(minIdx + 2)..(minIdx + 4)], 0);
            Comms comms = (Comms)(byte)((networkPacketData[minIdx + 4] & 0x80) >> 7);
            Ack ack = (Ack)(byte)((networkPacketData[minIdx + 4] & 0x60) >> 5);
            Encryption encryption = (Encryption)(networkPacketData[minIdx + 4] & 0x07);
            Cmd command = (Cmd)BitConverter.ToUInt16(new byte[] { networkPacketData[minIdx + 6], networkPacketData[minIdx + 5] });
            byte[] payload = networkPacketData[(minIdx + 7)..(maxIdx - 2)];
            byte[] crc = networkPacketData[(maxIdx - 2).. maxIdx];
            CmdAttribute commandDetails = null;

            if (_commandAttributes.ContainsKey(command))
                commandDetails = _commandAttributes[command];

            // crc isn't valid
            if (!ValidateBody(networkPacketData[idx..(idx + djiHeader.Size)])) return null;
            // an invalid command has been received
            else if(commandDetails == null)
            {
                Trace.TraceWarning($"Valid but unknown command received. " +
                    $"{sender} -> {receiver} {comms} {ack} {encryption} | " +
                    $"{networkPacketData[minIdx + 6].ToHexString()} " +
                    $"{networkPacketData[minIdx + 5].ToHexString()}");

                return null;
            }

            return new DjiPayload(sender, senderIndex, receiver, receiverIndex, counter, comms, ack, encryption, command, commandDetails, payload, crc);
        }

        internal static byte[] ConvertToBytes(DjiWifiHeader djiWifiHeader)
        {
            byte[] data = new byte[2 + djiWifiHeader.Session.Length + djiWifiHeader.Payload.Length];

            data[0] = (byte)djiWifiHeader.Size;
            data[1] = djiWifiHeader.Protocol;

            Array.Copy(djiWifiHeader.Session, 0, data, 2, djiWifiHeader.Session.Length);
            Array.Copy(djiWifiHeader.Payload, 0, data, 2 + djiWifiHeader.Session.Length, djiWifiHeader.Payload.Length);

            return data;
        }

        internal static byte[] ConvertToBytes(DjiHeader djiHeader)
        {
            ushort version = (ushort)(djiHeader.Version << 10);
            ushort sum = (ushort)(version + djiHeader.Size);

            return new byte[]
            {
                djiHeader.Delimiter,
                (byte)sum,
                (byte)(sum >> 8),
                djiHeader.CRC,
            };
        }

        internal static byte[] ConvertToBytes(DjiPayload djiPayload)
        {
            byte senderPlusIndex = (byte)((djiPayload.SenderIndex << 5) + djiPayload.Sender);
            byte receiverPlusIndex = (byte)((djiPayload.ReceiverIndex << 5) + djiPayload.Receiver);
            byte comms = (byte)((byte)djiPayload.Comms << 7);
            byte ack = (byte)((byte)djiPayload.Ack << 5);
            byte enc = (byte)((byte)djiPayload.Encryption);

            byte[] data = new byte[7 + djiPayload.Payload.Length + djiPayload.CRC.Length];

            data[0] = senderPlusIndex;
            data[1] = receiverPlusIndex;
            data[2] = (byte)djiPayload.Counter;
            data[3] = (byte)(djiPayload.Counter >> 8);
            data[4] = (byte)(comms + ack + enc);
            data[5] = djiPayload.CommandDetails.CmdSet;
            data[6] = djiPayload.CommandDetails.Cmd;

            Array.Copy(djiPayload.Payload, 0, data, 7, djiPayload.Payload.Length);
            Array.Copy(djiPayload.CRC, 0, data, 7 + djiPayload.Payload.Length, djiPayload.CRC.Length);

            return data;
        }
    }
}