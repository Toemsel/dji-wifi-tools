using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Controls.Filters
{
    public class IpAddressFilter : FilterControl
    {
        public IpAddressFilter() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
