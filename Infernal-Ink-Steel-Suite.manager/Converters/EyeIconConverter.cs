using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace InfernalInkSteelSuite.Converters
{
    public class EyeIconConverter : IValueConverter
    {
        private static readonly BitmapImage EyeOpen = new(new Uri("pack://application:,,,/Assets/Images/eye_open.png", UriKind.Absolute));
        private static readonly BitmapImage EyeClosed = new(new Uri("pack://application:,,,/Assets/Images/eye_closed.png", UriKind.Absolute));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? EyeOpen : EyeClosed;
            }
            return EyeClosed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
