using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dji.UI.View.DataGrid
{
    public class DjiNetworkPacketPoolInfo : UserControl
    {
        public DjiNetworkPacketPoolInfo() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
