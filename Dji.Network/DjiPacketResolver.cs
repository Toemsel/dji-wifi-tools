using Dji.Network.Packet;
using Dji.Network.Packet.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Dji.Network
{
    public delegate void DjiNetworkPacketReceived<V>(DjiNetworkPacket<V> packet) where V : DjiPacket;

    public delegate void DjiNetworkPacketReceived(DjiNetworkPacket packet);

    public class DjiPacketResolver
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _packetListeners = new();

        internal List<Delegate> this[Type packetType]
        {
            get
            {
                if (!_packetListeners.ContainsKey(packetType))
                    return new List<Delegate>();

                return _packetListeners[packetType];
            }
        }

        public void AddDjiPacketListener(DjiNetworkPacketReceived del)
        {
            Type packetType = typeof(DjiNetworkPacket);

            if (this[packetType].Contains(del))
                return;

            if (!_packetListeners.ContainsKey(packetType))
            {
                while (!_packetListeners.ContainsKey(packetType) &&
                    !_packetListeners.TryAdd(packetType, new List<Delegate>())) { }
            }

            _packetListeners[packetType].Add(del);
        }

        public void AddDjiPacketListener<T, V>(DjiNetworkPacketReceived<V> del) where T : DjiNetworkPacket<V> where V : DjiPacket
        {
            Type packetType = typeof(T);

            if (this[packetType].Contains(del))
                return;

            if(!_packetListeners.ContainsKey(packetType))
            {
                while (!_packetListeners.ContainsKey(packetType) &&
                    !_packetListeners.TryAdd(packetType, new List<Delegate>())) { }
            }

            _packetListeners[packetType].Add(del);
        }

        public void RemoveDjiPacketListener<T, V>(DjiNetworkPacketReceived<V> del) where T : DjiNetworkPacket<V> where V : DjiPacket
        {
            Type packetType = typeof(T);

            if (!_packetListeners.ContainsKey(packetType) || del == null) return;
            else if (!_packetListeners[packetType].Contains(del)) return;
            while(_packetListeners[packetType].Contains(del) && !_packetListeners[packetType].Remove(del)) { }
        }

        public void Feed(NetworkPacket networkPacket)
        {
            if (!networkPacket.UdpPacket.HasPayload)
            {
                Trace.TraceError($"Invalid byte stream received. Packet dropped");
            }
            else
            {
                foreach(var subPacket in networkPacket.ToDjiNetworkPackets())
                {
                    var djiPacketGeneric = typeof(DjiNetworkPacket);

                    for (int currentInvocationIndex = this[djiPacketGeneric].Count - 1; currentInvocationIndex >= 0; currentInvocationIndex--)
                        this[djiPacketGeneric][currentInvocationIndex].DynamicInvoke(new object[] { subPacket });
                }

                //var packetIdentifier = networkPacket.UdpPacket.Payload[0];
                //var djiPacketGeneric = typeof(DjiNetworkPacket<DjiUnknownPacket>);

                //if (_packetTypes.ContainsKey(packetIdentifier))
                //    djiPacketGeneric = typeof(DjiNetworkPacket<>).MakeGenericType(_packetTypes[packetIdentifier]);

                //var djiPacket = Activator.CreateInstance(djiPacketGeneric, new object[] { networkPacket });

                //for (int currentInvocationIndex = this[djiPacketGeneric].Count - 1; currentInvocationIndex >= 0; currentInvocationIndex--)
                //    this[djiPacketGeneric][currentInvocationIndex].DynamicInvoke(new object[] { djiPacket });

                //djiPacketGeneric = typeof(DjiNetworkPacket);
                //djiPacket = (DjiNetworkPacket)djiPacket;

                //for (int currentInvocationIndex = this[djiPacketGeneric].Count - 1; currentInvocationIndex >= 0; currentInvocationIndex--)
                //    this[djiPacketGeneric][currentInvocationIndex].DynamicInvoke(new object[] { djiPacket });
            }
        }
    }
}