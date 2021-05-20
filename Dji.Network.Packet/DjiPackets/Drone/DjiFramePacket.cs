using Dji.Network.Packet.DjiPackets.Base;
using Dji.Network.Packet.Extensions;
using System;

namespace Dji.Network.Packet.DjiPackets.Drone
{
    public class DjiFramePacket : DjiPacket
    {
        private byte[] _frameData;

        public static readonly byte[] END_OF_FRAME_UPDATE_DELIMITER = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x09, 0x10 };

        public byte[] FrameData
        {
            get => _frameData;
            set => SetValue(ref _frameData, value);
        }

        protected override byte[] Build() => throw new NotSupportedException($"Images may only be build by the drone itself");

        protected override bool Build(byte[] data)
        {
            if (!data.EndsWith(END_OF_FRAME_UPDATE_DELIMITER)) return false;

            FrameData = data[..^END_OF_FRAME_UPDATE_DELIMITER.Length];
            return true;
        }
    }
}