using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace InfernalInkSteelSuite.Services
{
    public enum ThemeId
    {
        Dark,
        Light,
        InfernalNeon,
        Oceanic,
        Forest,
        Vampire,
        EightiesSunset,
        Monochrome
    }

    public sealed class ThemeDefinition
    {
        public ThemeId Id { get; }
        public string Key { get; }
        public string DisplayName { get; }
        public Uri ResourceUri { get; }

        public ThemeDefinition(ThemeId id, string key, string displayName, string resourcePath)
        {
            Id = id;
            Key = key;
            DisplayName = displayName;
            ResourceUri = new Uri($"pack://application:,,,/{resourcePath}", UriKind.Absolute);
        }
    }

    public static class ThemeManager
    {
        public static IReadOnlyList<ThemeDefinition> AvailableThemes { get; } =
            new List<ThemeDefinition>
            {
                new ThemeDefinition(
                    ThemeId.Dark,
                    key: "Dark",
                    displayName: "Dark",
                    resourcePath: "Themes/Dark.xaml"),

                new ThemeDefinition(
                    ThemeId.Light,
                    key: "Light",
                    displayName: "Light",
                    resourcePath: "Themes/Light.xaml"),

                new ThemeDefinition(
                    ThemeId.InfernalNeon,
                    key: "InfernalNeon",
                    displayName: "Infernal Neon",
                    resourcePath: "Themes/InfernalNeon.xaml"),

                new ThemeDefinition(
                    ThemeId.Oceanic,
                    key: "Oceanic",
                    displayName: "Oceanic",
                    resourcePath: "Themes/Oceanic.xaml"),

                new ThemeDefinition(
                    ThemeId.Forest,
                    key: "Forest",
                    displayName: "Forest",
                    resourcePath: "Themes/Forest.xaml"),

                new ThemeDefinition(
                    ThemeId.Vampire,
                    key: "Vampire",
                    displayName: "Vampire",
                    resourcePath: "Themes/Vampire.xaml"),

                new ThemeDefinition(
                    ThemeId.EightiesSunset,
                    key: "80sSunset",
                    displayName: "80s Sunset",
                    resourcePath: "Themes/80sSunset.xaml"),

                new ThemeDefinition(
                    ThemeId.Monochrome,
                    key: "Monochrome",
                    displayName: "Monochrome",
                    resourcePath: "Themes/Monochrome.xaml"),
            };

        public static ThemeDefinition CurrentTheme { get; private set; } =
            AvailableThemes.First(t => t.Id == ThemeId.InfernalNeon);

        public static void ApplyTheme(ThemeId id)
        {
            var theme = AvailableThemes.First(t => t.Id == id);
            ApplyTheme(theme);
        }

        public static void ApplyTheme(string themeKey)
        {
            var theme = AvailableThemes.FirstOrDefault(
                t => string.Equals(t.Key, themeKey, StringComparison.OrdinalIgnoreCase))
                ?? AvailableThemes.First(t => t.Id == ThemeId.InfernalNeon);

            ApplyTheme(theme);
        }

        private static void ApplyTheme(ThemeDefinition theme)
        {
            var app = Application.Current;
            if (app is null)
                return;

            var dictionaries = app.Resources.MergedDictionaries;

            var toRemove = dictionaries
                .Where(d => d.Source != null &&
                            !d.Source.OriginalString.EndsWith("Base.xaml",
                                StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var dict in toRemove)
                dictionaries.Remove(dict);

            var themeDict = new ResourceDictionary
            {
                Source = theme.ResourceUri
            };
            dictionaries.Add(themeDict);

            CurrentTheme = theme;
        }
    }
}
