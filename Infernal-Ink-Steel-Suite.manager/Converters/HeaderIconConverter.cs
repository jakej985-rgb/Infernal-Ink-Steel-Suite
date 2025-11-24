using System;
using System.Globalization;
using System.Windows.Data;

namespace InfernalInkSteelSuite.Converters
{
    public class HeaderIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string header)
            {
                return header.ToLower() switch
                {
                    "user management" or "users" => "👥",
                    "admin" => "🔐",
                    "profile" => "👤",
                    "appearance & theme" or "theme" => "🎨",
                    "security" => "🔒",
                    "shop settings" or "settings" => "⚙️",
                    "shop hours" or "hours" => "🕐",
                    "login screen branding" or "branding" => "🖼️",
                    "application settings" => "📱",
                    "notifications" => "🔔",
                    "backup & data" or "backup" => "💾",
                    "accessibility" => "♿",
                    "integrations" or "linked accounts" => "🔗",
                    _ => "▶"
                };
            }
            return "▶";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
