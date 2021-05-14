using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia;
using System;
using System.ComponentModel;
using Dji.UI.ViewModels;

namespace Dji.UI
{
    public class DjiHexComparisonWindow : Window
    {
        private bool _isClosing = false;

        public DjiHexComparisonWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isClosing) return;

            _isClosing = true;
            ((DjiHexComparisonWindowViewModel)DataContext).OnControlWindowClose();
            base.OnClosing(e);
        }
    }
}