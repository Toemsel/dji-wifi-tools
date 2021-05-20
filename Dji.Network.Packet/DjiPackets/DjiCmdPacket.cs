using Dji.Network.Packet.DjiPackets.Base;
using Dji.Network.Packet.Extensions;
using System.Diagnostics;
using System;

namespace Dji.Network.Packet.DjiPackets
{
    public class DjiCmdPacket : DjiDUMLPacket
    {
        private Transceiver _sender;
        private byte _senderIndex;
        private Transceiver _receiver;
        private byte _receiverIndex;
        private ushort _counter;
        private Comms _comms;
        private Ack _ack;
        private Encryption _encryption;
        private Cmd _cmd;
        private CmdAttribute _cmdAttr;
        private byte[] _payload;
        private byte[] _crc;

        public DjiCmdPacket() { }

        public Transceiver Sender
        {
            get => _sender;
            set => SetValue(ref _sender, value);
        }

        public byte SenderIndex
        {
            get => _senderIndex;
            set => SetValue(ref _senderIndex, value);
        }

        public Transceiver Receiver
        {
            get => _receiver;
            set => SetValue(ref _receiver, value);
        }

        public byte ReceiverIndex
        {
            get => _receiverIndex;
            set => SetValue(ref _receiverIndex, value);
        }

        public ushort Counter
        {
            get => _counter;
            set => SetValue(ref _counter, value);
        }

        public Comms Comms
        {
            get => _comms;
            set => SetValue(ref _comms, value);
        }

        public Ack Ack
        {
            get => _ack;
            set => SetValue(ref _ack, value);
        }

        public Encryption Encryption
        {
            get => _encryption;
            set => SetValue(ref _encryption, value);
        }

        public Cmd Command
        {
            get => _cmd;
            set => SetValue(ref _cmd, value);
        }

        public CmdAttribute CommandDetails
        {
            get => _cmdAttr;
            set => SetValue(ref _cmdAttr, value);
        }

        public byte[] Payload
        {
            get => _payload;
            set => SetValue(ref _payload, value);
        }

        public byte[] CmdCrc
        {
            get => _crc;
            set => SetValue(ref _crc, value);
        }

        protected override bool Build(byte[] data)
        {
            if (!base.Build(data))
                return false;

            // as the parent did return 'true' after his build,
            // we know that the DumlSize has been set. Thus,
            // we can extract the actual payload from the data.
            byte[] dataPayload = data[HEADER_SIZE..DumlSize];

            byte senderIndex = (byte)((dataPayload[0] & 0xE0) >> 5);
            Transceiver sender = (Transceiver)(dataPayload[0] & 0x1f);
            byte receiverIndex = (byte)((dataPayload[1] & 0xE0) >> 5);
            Transceiver receiver = (Transceiver)(dataPayload[1] & 0x1f);
            ushort counter = BitConverter.ToUInt16(dataPayload[2..4], 0);
            Comms comms = (Comms)(byte)((dataPayload[4] & 0x80) >> 7);
            Ack ack = (Ack)(byte)((dataPayload[4] & 0x60) >> 5);
            Encryption encryption = (Encryption)(dataPayload[4] & 0x07);
            Cmd command = (Cmd)BitConverter.ToUInt16(new byte[] { dataPayload[6], dataPayload[5] });
            CmdAttribute commandDetails = CmdAttribute.TryGetAttribute(command);
            byte[] payload = dataPayload[7..^2];
            byte[] crc = dataPayload[^2..];

            // crc isn't valid
            if (!DjiCrc.Crc16(data[0..(DumlSize - 2)], crc)) return false;
            // an invalid command has been received
            else if (commandDetails == null)
            {
                Trace.TraceWarning($"Valid but unknown command received. " +
                    $"{sender} -> {receiver} {comms} {ack} {encryption} | " +
                    $"{dataPayload[6].ToHexString()} " +
                    $"{dataPayload[5].ToHexString()}");

                return false;
            }

            // as all parameters are valid, we can set the object values
            SenderIndex = senderIndex;
            Sender = sender;
            ReceiverIndex = receiverIndex;
            Receiver = receiver;
            Counter = counter;
            Comms = comms;
            Ack = ack;
            Encryption = encryption;
            Command = command;
            CommandDetails = commandDetails;
            Payload = payload;
            CmdCrc = crc;

            return true;
        }

        protected override byte[] Build()
        {
            byte senderPlusIndex = (byte)((SenderIndex << 5) + Sender);
            byte receiverPlusIndex = (byte)((ReceiverIndex << 5) + Receiver);
            byte comms = (byte)((byte)Comms << 7);
            byte ack = (byte)((byte)Ack << 5);
            byte enc = (byte)((byte)Encryption);

            byte[] data = new byte[7 + Payload.Length + CmdCrc.Length];

            data[0] = senderPlusIndex;
            data[1] = receiverPlusIndex;
            data[2] = (byte)Counter;
            data[3] = (byte)(Counter >> 8);
            data[4] = (byte)(comms + ack + enc);
            data[5] = CommandDetails.CmdSet;
            data[6] = CommandDetails.Cmd;

            Array.Copy(Payload, 0, data, 7, Payload.Length);
            Array.Copy(CmdCrc, 0, data, 7 + Payload.Length, CmdCrc.Length);

            return base.Build().Append(data);
        }
    }
}
