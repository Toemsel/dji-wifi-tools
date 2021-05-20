using Dji.Network.Packet.DjiPackets;
using Dji.Network.Packet;

namespace Dji.UI.ViewModels.Docks
{
    public class DjiTrafficDockViewModel : TrafficDockViewModel
    {
        public DjiTrafficDockViewModel()
        {
            DjiContentViewModel.Instance.OperatorPacketResolver.AddDjiPacketListener<DjiNetworkPacket<DjiCmdPacket>, DjiCmdPacket>(djiNetworkPacket => Store(djiNetworkPacket));
            DjiContentViewModel.Instance.DronePacketResolver.AddDjiPacketListener<DjiNetworkPacket<DjiCmdPacket>, DjiCmdPacket>(djiNetworkPacket => Store(djiNetworkPacket));
        }
    }
}