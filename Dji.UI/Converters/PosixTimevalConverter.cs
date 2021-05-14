using Avalonia.Data.Converters;
using System.Globalization;
using Dji.Network.Packet;
using System;
using SharpPcap;

namespace Dji.UI.Converters
{
    public class PosixTimevalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"{((PosixTimeval)value).Seconds}.{((PosixTimeval)value).MicroSeconds}";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
