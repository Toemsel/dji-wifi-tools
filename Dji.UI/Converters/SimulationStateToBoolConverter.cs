using Avalonia.Data.Converters;
using System.Globalization;
using Dji.Network;
using System;

namespace Dji.UI.Converters
{
    public class SimulationStateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string simulation && value is SimulationState state)
                return state == (SimulationState)Enum.Parse(typeof(SimulationState), simulation);

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
