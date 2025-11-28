using System;
using System.Globalization;
using System.Windows.Data;

namespace InfernalInkSteelSuite.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return i.ToString(culture);
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                if (int.TryParse(s, NumberStyles.Any, culture, out int result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}
