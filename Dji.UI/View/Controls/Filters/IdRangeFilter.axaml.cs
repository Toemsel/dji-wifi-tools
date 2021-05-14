using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Controls.Filters
{
    public class IdRangeFilter : FilterControl
    {
        public IdRangeFilter() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
