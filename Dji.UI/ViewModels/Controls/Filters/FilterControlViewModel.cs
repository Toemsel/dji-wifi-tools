using Dji.Network;
using Dji.Network.Packet;
using Dji.UI.Pooling;
using ReactiveUI;
using System;
using System.Linq.Expressions;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public abstract class FilterControlViewModel : ReactiveObject
    {
        private DjiNetworkPacketPool _djiNetworkPacketPool;
        private DjiNetworkInformation _djiNetworkInformation;

        public FilterControlViewModel()
        {
            DjiContentViewModel.Instance.PacketSniffer.NetworkStatusChanged += (obj, data) => DjiNetworkInformation = data;
            this.WhenAnyValue(instance => instance.DjiNetworkPacketPool).Subscribe(networkPool => networkPool?.AddFilter(FilterExpression));
        }

        public DjiNetworkPacketPool DjiNetworkPacketPool
        {
            get => _djiNetworkPacketPool;
            set => this.RaiseAndSetIfChanged(ref _djiNetworkPacketPool, value);
        }

        protected DjiNetworkInformation DjiNetworkInformation
        {
            get => _djiNetworkInformation;
            set => this.RaiseAndSetIfChanged(ref _djiNetworkInformation, value);
        }

        protected abstract Expression<Func<NetworkPacket, bool>> FilterExpression { get; }
    }
}