using Dji.Network.Packet;

namespace Dji.UI.ViewModels.Docks
{
    public class DjiEmptyTrafficDockViewModel : TrafficDockViewModel
    {
        public DjiEmptyTrafficDockViewModel()
        {
            void HasOnlyWifiHeaderFilter(DjiNetworkPacket djiNetworkPacket)
            {
                if (!djiNetworkPacket.DjiPacket.IsKnown)
                    Store(djiNetworkPacket);
            }

            DjiContentViewModel.Instance.PacketResolver.AddDjiPacketListener(HasOnlyWifiHeaderFilter);
        }
    }
}