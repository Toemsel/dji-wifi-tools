using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Docks
{
    public class UdpTrafficDock : NetworkTrafficDock
    {
        public UdpTrafficDock() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}