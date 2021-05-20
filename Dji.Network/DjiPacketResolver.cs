using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dji.Network
{
    public delegate void DjiNetworkPacketReceived<V>(DjiNetworkPacket<V> packet) where V : DjiPacket;

    public delegate void DjiNetworkPacketReceived(DjiNetworkPacket packet);

    public delegate void NetworkPacketReceived(NetworkPacket packet);

    public abstract class DjiPacketResolver
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

        public void AddPacketListener(NetworkPacketReceived del) => AddPacketListener(typeof(NetworkPacket), del);

        public void AddDjiPacketListener(DjiNetworkPacketReceived del) => AddPacketListener(typeof(DjiNetworkPacket), del);

        public void AddDjiPacketListener<T, V>(DjiNetworkPacketReceived<V> del) where T : DjiNetworkPacket<V> where V : DjiPacket => AddPacketListener(typeof(T), del);

        private void AddPacketListener(Type packetType, Delegate del)
        {
            if (this[packetType].Contains(del))
                return;

            if (!_packetListeners.ContainsKey(packetType))
            {
                while (!_packetListeners.ContainsKey(packetType) &&
                    !_packetListeners.TryAdd(packetType, new List<Delegate>())) { }
            }

            _packetListeners[packetType].Add(del);
        }

        protected abstract void ProcessNetworkPacket(NetworkPacket networkPacket);

        protected void Resolve(NetworkPacket networkPacket)
        {
            var djiPacketGeneric = typeof(NetworkPacket);

            for (int currentInvocationIndex = this[djiPacketGeneric].Count - 1; currentInvocationIndex >= 0; currentInvocationIndex--)
                this[djiPacketGeneric][currentInvocationIndex].DynamicInvoke(new object[] { networkPacket });
        }

        protected void Resolve(DjiNetworkPacket djiNetworkPacket)
        {
            var djiPacketGeneric = typeof(DjiNetworkPacket);

            for (int currentInvocationIndex = this[djiPacketGeneric].Count - 1; currentInvocationIndex >= 0; currentInvocationIndex--)
                this[djiPacketGeneric][currentInvocationIndex].DynamicInvoke(new object[] { djiNetworkPacket });

            Resolve(djiNetworkPacket as NetworkPacket);
        }

        protected void Resolve<T>(DjiNetworkPacket<T> djiNetworkPacket) where T : DjiPacket
        {
            var djiPacketGeneric = typeof(DjiNetworkPacket<T>);

            for (int currentInvocationIndex = this[djiPacketGeneric].Count - 1; currentInvocationIndex >= 0; currentInvocationIndex--)
                this[djiPacketGeneric][currentInvocationIndex].DynamicInvoke(new object[] { djiNetworkPacket });

            // resolve again as a common NetworkPacket; this enables
            // all subscribers (who only want DjiPackets) to receive it
            Resolve(djiNetworkPacket as DjiNetworkPacket);
        }

        public void Feed(NetworkPacket networkPacket) => ProcessNetworkPacket(networkPacket);
    }
}