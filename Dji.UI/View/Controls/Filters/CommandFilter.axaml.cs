using Avalonia.Markup.Xaml;

namespace Dji.UI.View.Controls.Filters
{
    public partial class CommandFilter : FilterControl
    {
        public CommandFilter() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}