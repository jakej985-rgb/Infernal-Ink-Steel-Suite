using System;
using System.Globalization;
using System.Windows.Data;

namespace InfernalInkSteelSuite.UI.Converters
{
    public class ValidationResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isValid)
            {
                return isValid ? "✓" : "❌";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
