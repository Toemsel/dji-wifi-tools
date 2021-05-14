using Avalonia.Data.Converters;
using Dji.Network.Packet.Extensions;
using System;
using System.Globalization;

namespace Dji.UI.Converters
{
    public class ByteToStringConverter : IValueConverter
    {
        public bool UseSpacing { get; set; } = true;

        public bool UseLeadingZero { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((byte[])value).ToHexString(UseLeadingZero, UseSpacing);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
