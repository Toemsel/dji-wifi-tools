using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dji.UI.ViewModels;
using System;

namespace Dji.UI
{
    public class DjiWindow : Window
    {
        private static Lazy<DjiWindow> _singleton = new Lazy<DjiWindow>(() => new());

        public DjiWindow() => InitializeComponent();

        public static DjiWindow Instance => _singleton.Value;

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        protected override void OnOpened(EventArgs e) => DjiContentViewModel.Instance.OnApplicationEnter();

        protected override void OnClosed(EventArgs e) => DjiContentViewModel.Instance.OnApplicationExit();
    }
}