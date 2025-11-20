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
        Monochrome,
        Christmas,
        Halloween,
        July4th,
        Thanksgiving,
        Easter,
        MothersDay,
        FathersDay,
        Spring,
        Summer,
        Winter,
        Fall
    }

    public sealed class ThemeDefinition(ThemeId id, string key, string displayName, string resourcePath)
    {
        public ThemeId Id { get; } = id;
        public string Key { get; } = key;
        public string DisplayName { get; } = displayName;
        public Uri ResourceUri { get; } = new Uri($"pack://application:,,,/{resourcePath}", UriKind.Absolute);
    }

    public static class ThemeManager
    {
        public static IReadOnlyList<ThemeDefinition> AvailableThemes { get; } =
            [
                new(
                    ThemeId.Dark,
                    key: "Dark",
                    displayName: "Dark",
                    resourcePath: "Themes/Dark.xaml"),

                new(
                    ThemeId.Light,
                    key: "Light",
                    displayName: "Light",
                    resourcePath: "Themes/Light.xaml"),

                new(
                    ThemeId.InfernalNeon,
                    key: "InfernalNeon",
                    displayName: "Infernal Neon",
                    resourcePath: "Themes/InfernalNeon.xaml"),

                new(
                    ThemeId.Oceanic,
                    key: "Oceanic",
                    displayName: "Oceanic",
                    resourcePath: "Themes/Oceanic.xaml"),

                new(
                    ThemeId.Forest,
                    key: "Forest",
                    displayName: "Forest",
                    resourcePath: "Themes/Forest.xaml"),

                new(
                    ThemeId.Vampire,
                    key: "Vampire",
                    displayName: "Vampire",
                    resourcePath: "Themes/Vampire.xaml"),

                new(
                    ThemeId.EightiesSunset,
                    key: "80sSunset",
                    displayName: "80s Sunset",
                    resourcePath: "Themes/80sSunset.xaml"),

                new(
                    ThemeId.Monochrome,
                    key: "Monochrome",
                    displayName: "Monochrome",
                    resourcePath: "Themes/Monochrome.xaml"),
                new(
                    ThemeId.Christmas,
                    key: "Christmas",
                    displayName: "Christmas",
                    resourcePath: "Themes/Christmas.xaml"),
                new(
                    ThemeId.Halloween,
                    key: "Halloween",
                    displayName: "Halloween",
                    resourcePath: "Themes/Halloween.xaml"),
                new(
                    ThemeId.July4th,
                    key: "July4th",
                    displayName: "4th of July",
                    resourcePath: "Themes/July4th.xaml"),
                new(
                    ThemeId.Thanksgiving,
                    key: "Thanksgiving",
                    displayName: "Thanksgiving",
                    resourcePath: "Themes/Thanksgiving.xaml"),
                new(
                    ThemeId.Easter,
                    key: "Easter",
                    displayName: "Easter",
                    resourcePath: "Themes/Easter.xaml"),
                new(
                    ThemeId.MothersDay,
                    key: "MothersDay",
                    displayName: "Mother's Day",
                    resourcePath: "Themes/MothersDay.xaml"),
                new(
                    ThemeId.FathersDay,
                    key: "FathersDay",
                    displayName: "Father's Day",
                    resourcePath: "Themes/FathersDay.xaml"),
                new(
                    ThemeId.Spring,
                    key: "Spring",
                    displayName: "Spring",
                    resourcePath: "Themes/Spring.xaml"),
                new(
                    ThemeId.Summer,
                    key: "Summer",
                    displayName: "Summer",
                    resourcePath: "Themes/Summer.xaml"),
                new(
                    ThemeId.Winter,
                    key: "Winter",
                    displayName: "Winter",
                    resourcePath: "Themes/Winter.xaml"),
                new(
                    ThemeId.Fall,
                    key: "Fall",
                    displayName: "Fall",
                    resourcePath: "Themes/Fall.xaml"),
            ];

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
