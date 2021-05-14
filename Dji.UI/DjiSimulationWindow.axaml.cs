using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dji.UI.ViewModels;
using System.ComponentModel;

namespace Dji.UI
{
    public class DjiSimulationWindow : Window
    {
        public DjiSimulationWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosing(CancelEventArgs e) => (DataContext as DjiSimulationWindowViewModel)?.Dispose();
    }
}
