using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Docks
{
    public class DjiEmptyTrafficDock : NetworkTrafficDock
    {
        public DjiEmptyTrafficDock() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}