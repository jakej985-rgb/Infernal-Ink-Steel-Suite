using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace InfernalInkSteelSuite.UI.Converters
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "scheduled" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")), // Green
                    "confirmed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")), // Green
                    "pending" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107")),   // Amber
                    "completed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E9E9E")), // Gray
                    "cancelled" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")), // Red
                    "no show" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")),   // Red
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")),           // Blue
                };
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
