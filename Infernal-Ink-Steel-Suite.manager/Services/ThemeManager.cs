using System.Collections.Generic;
using System.Windows.Media;

namespace InfernalInkSteelSuite.Services
{
    public class Theme
    {
        public string Name { get; set; } = string.Empty;
        public Color PrimaryColor { get; set; }
        public Color SecondaryColor { get; set; }
        public Color TextColor { get; set; }
    }

    public class ThemeManager
    {
        private static ThemeManager? _instance;
        public static ThemeManager Instance => _instance ??= new ThemeManager();

        public List<Theme> Themes { get; }
        public Theme CurrentTheme { get; private set; }

        private ThemeManager()
        {
            Themes = new List<Theme>
            {
                new Theme { Name = "Default", PrimaryColor = (Color)ColorConverter.ConvertFromString("#FF00FFFF"), SecondaryColor = (Color)ColorConverter.ConvertFromString("#333333"), TextColor = Colors.White },
                new Theme { Name = "Dark", PrimaryColor = (Color)ColorConverter.ConvertFromString("#FFFF0000"), SecondaryColor = (Color)ColorConverter.ConvertFromString("#000000"), TextColor = Colors.White },
                new Theme { Name = "Light", PrimaryColor = (Color)ColorConverter.ConvertFromString("#FF0000FF"), SecondaryColor = (Color)ColorConverter.ConvertFromString("#FFFFFF"), TextColor = Colors.Black }
            };
            CurrentTheme = Themes[0];
        }

        public event System.Action<Theme>? ThemeChanged;

        public void SetTheme(Theme theme)
        {
            CurrentTheme = theme;
            ThemeChanged?.Invoke(CurrentTheme);
        }
    }
}
