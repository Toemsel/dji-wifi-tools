using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dji.UI.View.StatusBar
{
    public class DjiStatusBar : UserControl
    {
        public DjiStatusBar() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
