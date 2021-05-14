using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Dji.UI.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateTime)value).ToString("mm:ss.ffff");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
