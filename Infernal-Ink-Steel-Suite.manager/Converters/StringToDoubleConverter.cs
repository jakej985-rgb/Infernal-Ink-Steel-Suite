using System;
using System.Globalization;
using System.Windows.Data;

namespace InfernalInkSteelSuite.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return d.ToString(culture);
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                if (double.TryParse(s, NumberStyles.Any, culture, out double result))
                {
                    return result;
                }
            }
            return 0.0;
        }
    }
}
