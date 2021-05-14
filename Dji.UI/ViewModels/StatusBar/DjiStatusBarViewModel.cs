namespace Dji.UI.ViewModels.StatusBar
{
    public class DjiStatusBarViewModel
    {
        private readonly DroneStatusViewModel _droneViewModel;

        public DjiStatusBarViewModel()
        {
            _droneViewModel = new(DjiContentViewModel.Instance.PacketSniffer);
        }

        public DroneStatusViewModel DroneViewModel => _droneViewModel;
    }
}