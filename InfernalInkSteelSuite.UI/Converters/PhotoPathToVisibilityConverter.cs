using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace InfernalInkSteelSuite.UI.Converters
{
    public class PhotoPathToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If the parameter is "HasPhoto", return Visible when photo exists, Collapsed otherwise.
        /// If the parameter is "NoPhoto", return Visible when photo doesn't exist, Collapsed otherwise.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var photoPath = value as string;
            var hasPhoto = !string.IsNullOrWhiteSpace(photoPath) && File.Exists(photoPath);

            var mode = parameter as string;
            if (mode == "HasPhoto")
            {
                return hasPhoto ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (mode == "NoPhoto")
            {
                return hasPhoto ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
