using Avalonia.Data;
using Dji.Network.Packet;
using Dji.Network.Packet.Extensions;
using Dji.UI.Extensions;
using ReactiveUI;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class HexFilterViewModel : FilterControlViewModel
    {
        private string _hex;

        public HexFilterViewModel() => this.WhenAnyValue(instance => instance.Hex).Subscribe(hex => DjiNetworkPacketPool?.EvaluateFilterOnPackets());

        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => (networkPacket) => string.IsNullOrWhiteSpace(Hex) || networkPacket.Payload.Contains(Hex);

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