using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets;

namespace Dji.UI.ViewModels.Docks
{
    public class DjiEmptyTrafficDockViewModel : TrafficDockViewModel
    {
        public DjiEmptyTrafficDockViewModel()
        {
            DjiContentViewModel.Instance.OperatorPacketResolver.AddDjiPacketListener<DjiNetworkPacket<DjiEmptyPacket>, DjiEmptyPacket>(djiNetworkPacket => Store(djiNetworkPacket));
            DjiContentViewModel.Instance.DronePacketResolver.AddDjiPacketListener<DjiNetworkPacket<DjiEmptyPacket>, DjiEmptyPacket>(djiNetworkPacket => Store(djiNetworkPacket));
        }
    }
}