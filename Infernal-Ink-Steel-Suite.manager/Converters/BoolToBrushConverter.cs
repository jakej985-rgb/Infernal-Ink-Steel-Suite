using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InfernalInkSteelSuite.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")); // Green
        public Brush FalseBrush { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")); // Red

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? TrueBrush : FalseBrush;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
