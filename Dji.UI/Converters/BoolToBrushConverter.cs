using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Dji.UI.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public IBrush True { get; set; }

        public IBrush False { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((bool)value) ? True : False;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((IBrush)value) == True;
    }
}
