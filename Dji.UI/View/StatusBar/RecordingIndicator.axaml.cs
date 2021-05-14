using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Dji.UI.View.StatusBar
{
    public class RecordingIndicator : UserControl
    {
        public RecordingIndicator()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
