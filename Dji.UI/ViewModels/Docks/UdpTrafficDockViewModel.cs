namespace Dji.UI.ViewModels.Docks
{
    public class UdpTrafficDockViewModel : TrafficDockViewModel
    {
        public UdpTrafficDockViewModel()
        {
            DjiContentViewModel.Instance.OperatorPacketResolver.AddPacketListener(packet => Store(packet));
            DjiContentViewModel.Instance.DronePacketResolver.AddPacketListener(packet => Store(packet));
        }
    }
}