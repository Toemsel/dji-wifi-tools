using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Dji.UI.View.StatusBar
{
    public class StatusBarItem : UserControl
    {
        private IImage _image;

        public static readonly DirectProperty<StatusBarItem, IImage> ImageProperty = AvaloniaProperty.RegisterDirect<StatusBarItem, IImage>(
            nameof(Image), u => u.Image, (u, i) => u.Image = i);

        public IImage Image
        {
            get => _image;
            set => SetAndRaise(ImageProperty, ref _image, value);
        }

        public StatusBarItem() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}