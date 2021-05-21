using Avalonia;
using Avalonia.Controls;
using Dji.UI.Pooling;

namespace Dji.UI.View.Docks
{
    public class NetworkTrafficDock : UserControl
    {
        private bool _isSelected;

        public static readonly DirectProperty<NetworkTrafficDock, bool> IsSelectedProperty = AvaloniaProperty.RegisterDirect<NetworkTrafficDock, bool>(
            nameof(IsSelected), u => u.IsSelected, (u, i) => u.IsSelected = i);

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (DataContext is DjiNetworkPacketPool networkPool)
                    networkPool.FreezeNetworkPackets = !value;

                SetAndRaise(IsSelectedProperty, ref _isSelected, value);
            }
        }
    }
}