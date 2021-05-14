using System.Linq.Expressions;
using Dji.Network.Packet;
using Dji.Network;
using ReactiveUI;
using System;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class IpAddressFilterViewModel : FilterControlViewModel
    {
        private bool _operator;
        private bool _drone;

        public IpAddressFilterViewModel()
        {
            this.WhenAnyValue(instance => instance.Drone).Subscribe(drone => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.Operator).Subscribe(op => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
        }

        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => (networkPacket) =>
            (!Drone && !Operator) ||
            (networkPacket.UdpPacket.SourceIpAddress == DroneIpAddress && Drone) ||
            (networkPacket.UdpPacket.SourceIpAddress == OperatorIpAddress && Operator);

        private string DroneIpAddress => DjiNetworkInformation?.DroneIpAddress ?? string.Empty;
        private string OperatorIpAddress => DjiNetworkInformation?.OperatorIpAddress ?? string.Empty;

        public bool Drone
        {
            get => _drone;
            set => this.RaiseAndSetIfChanged(ref _drone, value);
        }

        public bool Operator
        {
            get => _operator;
            set => this.RaiseAndSetIfChanged(ref _operator, value);
        }
    }
}