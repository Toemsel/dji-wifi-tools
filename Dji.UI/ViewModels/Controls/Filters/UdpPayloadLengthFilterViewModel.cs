using System.Linq.Expressions;
using Dji.Network.Packet;
using System;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class UdpPayloadLengthFilterViewModel : NumbericFilterViewModel
    {
        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => networkPacket => !HasValue || networkPacket.UdpPacket.Payload.Length == Numeric;
    }
}
