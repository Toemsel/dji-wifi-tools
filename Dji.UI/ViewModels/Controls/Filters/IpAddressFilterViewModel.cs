using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets.Base;
using ReactiveUI;
using System;
using System.Linq.Expressions;

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
            (networkPacket.Participant == Participant.Drone && Drone) ||
            (networkPacket.Participant == Participant.Operator && Operator);

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