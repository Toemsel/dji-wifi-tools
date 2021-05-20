using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets;
using Dji.Network.Packet.DjiPackets.Base;
using Dji.Network.Packet.DjiPackets.Drone;
using Dji.Network.Packet.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dji.Network
{
    public class DjiDronePacketResolver : DjiPacketResolver
    {
        private Queue<byte[]> _frameUpdates = new Queue<byte[]>();

        protected override void ProcessNetworkPacket(NetworkPacket networkPacket)
        {
            WhType whType = (WhType)networkPacket.Payload[0x06];

            // case 1.: Drone's handshake
            if (networkPacket.Payload[0] == 0x08 && networkPacket.Payload[1] == 0x80)
                _ = InstSetRes<DjiHandshakePacket>(networkPacket, djiPacket: out _, 0x04);
            // case 2.: Camera-feed update received
            else if (whType == WhType.DroneImgFrame)
                HandleFrameNetworkPacket(networkPacket);
            // case 3.: Command packet(s) received
            else if (whType == WhType.DroneCmd1 || whType == WhType.DroneCmd2)
                HandleCmdNetworkPacket(networkPacket);
            else Trace.TraceWarning($"Unknown drone {nameof(WhType)} received {networkPacket.Payload[0x06].ToHexString()}");
        }

        private bool InstSetRes<T>(NetworkPacket networkPacket, out T djiPacket, int? delimiter = null) where T : DjiPacket
        {
            djiPacket = Activator.CreateInstance<T>();
            bool djiPacketState = djiPacket.Set(networkPacket.Payload, delimiter);

            if (djiPacketState) 
                Resolve(networkPacket.Wrap<T>(djiPacket));

            return djiPacketState;
        }

        private void HandleCmdNetworkPacket(NetworkPacket networkPacket)
        {
            // the drone tends to send not one, but several commands at once.
            // they are aggregated and chained from head to tail. (or tail to head, whatever)

            int idx = DjiPacket.GetWhSize(networkPacket.Payload);

            while(idx < networkPacket.Payload.Length)
            {
                bool instSetRes = InstSetRes(networkPacket, out DjiCmdPacket packet, idx);

                if (instSetRes)
                {
                    // set the head of the next packet to the current tail
                    idx += packet.DumlSize;
                }
                else if (packet.DumlSize != default(ushort))
                {
                    // the command didn't resolve, but the chain is still intact
                    Trace.TraceWarning($"Faulty cmd received. Chain successfully recovered. " +
                        $"Faulty cmd {networkPacket.Payload[idx..(idx + packet.DumlSize)].ToHexString(false, false)}");

                    idx += packet.DumlSize;
                }
                else
                {
                    // the command didn't resolve and the chain is broken
                    Trace.TraceError($"Faulty cmd received. Chain broken. " +
                        $"Skip data {networkPacket.Payload[idx..].ToHexString(false, false)}");
                }

                // as we weren't able to reconstruct the cmd packet, drop the received packet
                if (!instSetRes) return;
            }
        }

        private void HandleFrameNetworkPacket(NetworkPacket networkPacket)
        {
            int frameStartIdx = DjiPacket.GetWhSize(networkPacket.Payload);

            _frameUpdates.Enqueue(networkPacket.Payload[frameStartIdx..]);

            // we did receive a frame-update. however, we didn't reach the end of the
            // frame-update yet. Thus, remember the data and wait for more to come
            if (!networkPacket.Payload.EndsWith(DjiFramePacket.END_OF_FRAME_UPDATE_DELIMITER)) return;

            byte[] frameUpdateData = networkPacket.Payload[..frameStartIdx];
            frameUpdateData = frameUpdateData.Append(_frameUpdates.ToArray().SelectMany(s => s).ToArray());

            DjiFramePacket framePacket = new DjiFramePacket();
            if (framePacket.Set(frameUpdateData, frameStartIdx))
                Resolve(networkPacket.Wrap<DjiFramePacket>(framePacket));

            // reset our frame-buffer
            _frameUpdates = new Queue<byte[]>();
        }
    }
}