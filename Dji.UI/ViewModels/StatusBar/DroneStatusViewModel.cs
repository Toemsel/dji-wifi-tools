using System.Diagnostics.CodeAnalysis;
using Dji.Network.Packet;
using Dji.Network;
using System;

namespace Dji.UI.ViewModels.StatusBar
{
    public class DroneStatusViewModel : StatusBarItemViewModel
    {
        public DroneStatusViewModel([NotNull] DjiPacketSniffer packetSniffer) => packetSniffer.NetworkStatusChanged += (obj, data) => Status = data.Status == NetworkStatus.Connected;

        protected override bool CanToggle => false;

        public override void Off() => throw new NotSupportedException();

        public override void On() => throw new NotSupportedException();
    }
}