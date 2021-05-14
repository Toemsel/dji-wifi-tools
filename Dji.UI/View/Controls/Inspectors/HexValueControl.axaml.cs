using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Controls.Inspectors
{
    public class HexValueControl : UserControl
    {
        public HexValueControl() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
