using Dji.Network.Packet;

namespace Dji.UI.ViewModels.Docks
{
    public class DjiTrafficDockViewModel : TrafficDockViewModel
    {
        public DjiTrafficDockViewModel()
        {
            void HasDjiPayloadFilter(DjiNetworkPacket djiNetworkPacket)
            {
                if (djiNetworkPacket.DjiPacket.IsKnown)
                    Store(djiNetworkPacket);
            }

            DjiContentViewModel.Instance.PacketResolver.AddDjiPacketListener(HasDjiPayloadFilter);
        }
    }
}