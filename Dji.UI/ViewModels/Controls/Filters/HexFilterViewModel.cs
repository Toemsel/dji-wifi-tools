using System.Linq.Expressions;
using Dji.Network.Packet;
using Dji.UI.Extensions;
using Avalonia.Data;
using System.Linq;
using ReactiveUI;
using System;
using Dji.Network.Packet.Extensions;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class HexFilterViewModel : FilterControlViewModel
    {
        private string _hex;

        public HexFilterViewModel() => this.WhenAnyValue(instance => instance.Hex).Subscribe(hex => DjiNetworkPacketPool?.EvaluateFilterOnPackets());

        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => (networkPacket) => string.IsNullOrWhiteSpace(Hex) || networkPacket.UdpPacket.Payload.Contains(Hex);

        public string Hex
        {
            get => _hex;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.IsValidHexString())
                    throw new DataValidationException(string.Empty);

                this.RaiseAndSetIfChanged(ref _hex, value);
            }
        }
    }
}