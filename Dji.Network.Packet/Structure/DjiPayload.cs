namespace Dji.Network.Packet.Structure
{
    public class DjiPayload : IDjiData
    {
        private byte[] _data;

        internal DjiPayload(Transceiver sender, byte senderIndex, Transceiver receiver,
            byte receiverIndex, ushort counter, Comms comms, Ack ack, Encryption encryption,
            Cmd command, CmdAttribute commandDetails, byte[] payload, byte[] crc) =>
            (Sender, SenderIndex, Receiver, ReceiverIndex, Counter, Comms, Ack, Encryption, Command, CommandDetails, Payload, CRC) =
            (sender, senderIndex, receiver, receiverIndex, counter, comms, ack, encryption, command, commandDetails, payload, crc);

        public Transceiver Sender { get; init; }

        public byte SenderIndex { get; init; }

        public Transceiver Receiver { get; init; }

        public byte ReceiverIndex { get; init; }

        public ushort Counter { get; init; }

        public Comms Comms { get; init; }

        public Ack Ack { get; init; }

        public Encryption Encryption { get; init; }

        public Cmd Command { get; init; }

        public CmdAttribute CommandDetails { get; init; }

        public byte[] Payload { get; init; }

        public byte[] CRC { get; init; }

        public byte[] GetBytes() => _data ??= DjiFactory.ConvertToBytes(this);
    }
}