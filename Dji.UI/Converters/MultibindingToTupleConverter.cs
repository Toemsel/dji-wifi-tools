using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dji.UI.Converters
{
    public class MultibindingToTupleConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) => new Tuple<object, object>(values[0], values[1]);
    }
}
