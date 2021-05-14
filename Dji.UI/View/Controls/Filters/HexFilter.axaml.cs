using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Controls.Filters
{
    public class HexFilter : FilterControl
    {
        public HexFilter() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}