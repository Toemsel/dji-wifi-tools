using System.Linq.Expressions;
using Dji.Network.Packet;
using System;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class NetworkPacketIdFilterViewModel : NumbericFilterViewModel
    {
        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => networkPacket => !HasValue || networkPacket.Id == Numeric;
    }
}
