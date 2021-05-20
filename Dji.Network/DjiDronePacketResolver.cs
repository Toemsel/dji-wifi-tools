using Dji.Network.Packet;
using Dji.Network.Packet.Extensions;
using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dji.Network
{
    public enum ReadState
    {
        Init,
        Undefined,
        FixLength,
        EndOfStream,
        VideoFrame,
        NextPacket
    }

    public class DjiDronePacketResolver : DjiPacketResolver
    {
        private readonly byte[] _endOfStreamDelimiter = "0x000000010910".FromHexString();

        #region frame variables
        private Queue<byte[]> _frames = null;
        #endregion frame variables

        #region state variables
        private byte[] _dataCarry = null;
        private NetworkPacket _head = null;
        private byte[] _nextPacketIndicator = null;
        private ReadState _readState = ReadState.Init;
        private int _readLength = 0;
        #endregion state variables

        protected override void ProcessNetworkPacket(NetworkPacket networkPacket)
        {

        }
    }
}