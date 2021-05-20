using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets;
using Dji.Network.Packet.DjiPackets.Base;
using Dji.Network.Packet.DjiPackets.Drone;
using Dji.Network.Packet.Extensions;
using System;
using System.Diagnostics;

namespace Dji.Network
{
    public class DjiDronePacketResolver : DjiPacketResolver
    {
        protected override void ProcessNetworkPacket(NetworkPacket networkPacket)
        {
            // case 1.: Drone's handshake
            if (networkPacket.Payload[0] == 0x08 && networkPacket.Payload[1] == 0x80)
                _ = InstSetRes<DjiHandshakePacket>(networkPacket, djiPacket: out _, 0x04);
            // case 2.: Command packet(s)
            else if (IsCmdNetworkPacket(networkPacket))
                HandleCmdNetworkPacket(networkPacket);
            // case 3.: We deal with a frame-update
            else HandleFrameNetworkPacket(networkPacket);
        }

        private bool InstSetRes<T>(NetworkPacket networkPacket, out T djiPacket, int? delimiter = null) where T : DjiPacket
        {
            djiPacket = Activator.CreateInstance<T>();
            bool djiPacketState = djiPacket.Set(networkPacket.Payload, delimiter);

            if (djiPacketState) 
                Resolve(networkPacket.Wrap(djiPacket, typeof(T)));

            return djiPacketState;
        }

        private void HandleCmdNetworkPacket(NetworkPacket networkPacket)
        {
            // the drone tends to send not one, but several commands at once.
            // they are aggregated and chained from head to tail. (or tail to head, whatever)

            int idx = 32; // the very first packet starts at pos 32

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

        private bool IsCmdNetworkPacket(NetworkPacket networkPacket)
        {
            // if the packet isn't large enough, it cant be a cmd packet
            if (networkPacket.Payload.Length <= 0x20) return false;
            // if the delimiter valid, it cant be a cmd packet
            else if (networkPacket.Payload[32] != 0x55) return false;

            // it COULD be a cmd packet. It still depends on the data though.
            DjiCmdPacket djiCmdPacket = new DjiCmdPacket();
            // evaluate whether we deal with a Cmd-packet by creating one.
            return djiCmdPacket.Set(networkPacket.Payload, 32) || djiCmdPacket.DumlSize != default(ushort);
        }

        private void HandleFrameNetworkPacket(NetworkPacket networkPacket)
        {

        }
    }
}