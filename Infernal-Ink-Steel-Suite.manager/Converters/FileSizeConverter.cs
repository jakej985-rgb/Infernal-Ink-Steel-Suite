using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace InfernalInkSteelSuite.Converters
{
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    var fileInfo = new FileInfo(path);
                    long bytes = fileInfo.Length;

                    string[] sizes = ["B", "KB", "MB", "GB"];
                    double len = bytes;
                    int order = 0;

                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len /= 1024;
                    }

                    return $"{len:0.##} {sizes[order]}";
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
