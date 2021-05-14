namespace Dji.UI.ViewModels.Docks
{
    public class UdpTrafficDockViewModel : TrafficDockViewModel
    {
        public UdpTrafficDockViewModel() => DjiContentViewModel.Instance.PacketResolver.AddDjiPacketListener(packet => Store(packet));
    }
}
