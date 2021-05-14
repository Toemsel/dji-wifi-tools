using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Docks
{
    public class DjiTrafficDock : NetworkTrafficDock
    {
        public DjiTrafficDock() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}