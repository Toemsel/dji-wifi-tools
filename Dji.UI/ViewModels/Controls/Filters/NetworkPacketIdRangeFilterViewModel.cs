using Avalonia.Data;
using Dji.Network.Packet;
using ReactiveUI;
using System;
using System.Linq.Expressions;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class NetworkPacketIdRangeFilterViewModel : FilterControlViewModel
    {
        private string _packetRange;
        private int _fromPacketId;
        private int _toPacketId;

        public NetworkPacketIdRangeFilterViewModel() => this.WhenAnyValue(instance => instance.PacketRange).Subscribe(range => DjiNetworkPacketPool?.EvaluateFilterOnPackets());

        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => networkPacket => string.IsNullOrWhiteSpace(PacketRange) || networkPacket.Id >= _fromPacketId && networkPacket.Id <= _toPacketId;

        public int FromPacketId => _fromPacketId;

        public int ToPacketId => _toPacketId;

        public string PacketRange
        {
            get => _packetRange;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] numbers = value.Replace(" ", "").Split('-');

                    if (numbers.Length <= 1 ||
                        !int.TryParse(numbers[0], out _fromPacketId) ||
                        !int.TryParse(numbers[1], out _toPacketId))
                        throw new DataValidationException(string.Empty);
                }

                this.RaiseAndSetIfChanged(ref _packetRange, value);
            }
        }
    }
}