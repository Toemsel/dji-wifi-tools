using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Dji.UI
{
    public class DjiContent : UserControl
    {
        public DjiContent() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}