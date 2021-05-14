using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia;

namespace Dji.UI.View.Controls.Filters
{
    public class NumericFilter : FilterControl
    {
        private IImage _image;
        private string _description;

        public static readonly DirectProperty<NumericFilter, IImage> ImageProperty = AvaloniaProperty.RegisterDirect<NumericFilter, IImage>(
            nameof(Image), u => u.Image, (u, i) => u.Image = i);

        public static readonly DirectProperty<NumericFilter, string> DescriptionProperty = AvaloniaProperty.RegisterDirect<NumericFilter, string>(
            nameof(Description), u => u.Description, (u, i) => u.Description = i);

        public IImage Image
        {
            get => _image;
            set => SetAndRaise(ImageProperty, ref _image, value);
        }

        public string Description
        {
            get => _description;
            set => SetAndRaise(DescriptionProperty, ref _description, value);
        }

        public NumericFilter() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}