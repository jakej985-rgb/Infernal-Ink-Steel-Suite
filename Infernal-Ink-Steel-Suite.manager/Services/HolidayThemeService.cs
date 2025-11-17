using System;
using System.Collections.Generic;
using System.Linq;

namespace InfernalInkSteelSuite.Services
{
    public static class HolidayThemeService
    {
        private static readonly Dictionary<ThemeId, (DateTime Start, DateTime End)> HolidayRanges = new();

        static HolidayThemeService()
        {
            InitializeHolidayRanges(DateTime.Now.Year);
        }

        private static void InitializeHolidayRanges(int year)
        {
            HolidayRanges.Clear();

            // Holidays with fixed dates (3 weeks before)
            HolidayRanges.Add(ThemeId.Christmas, (new DateTime(year, 12, 25).AddDays(-21), new DateTime(year, 12, 26)));
            HolidayRanges.Add(ThemeId.July4th, (new DateTime(year, 7, 4).AddDays(-21), new DateTime(year, 7, 5)));
            HolidayRanges.Add(ThemeId.Halloween, (new DateTime(year, 10, 31).AddDays(-21), new DateTime(year, 11, 1)));

            // Holidays with variable dates (e.g., Thanksgiving, Easter)
            HolidayRanges.Add(ThemeId.Thanksgiving, (GetThanksgivingDate(year).AddDays(-21), GetThanksgivingDate(year).AddDays(1)));
            HolidayRanges.Add(ThemeId.Easter, (GetEasterDate(year).AddDays(-21), GetEasterDate(year).AddDays(1)));
            HolidayRanges.Add(ThemeId.MothersDay, (GetMothersDay(year).AddDays(-21), GetMothersDay(year).AddDays(1)));
            HolidayRanges.Add(ThemeId.FathersDay, (GetFathersDay(year).AddDays(-21), GetFathersDay(year).AddDays(1)));

            // Seasons
            HolidayRanges.Add(ThemeId.Spring, (new DateTime(year, 3, 20), new DateTime(year, 6, 20)));
            HolidayRanges.Add(ThemeId.Summer, (new DateTime(year, 6, 21), new DateTime(year, 9, 22)));
            HolidayRanges.Add(ThemeId.Fall, (new DateTime(year, 9, 23), new DateTime(year, 12, 21)));
            HolidayRanges.Add(ThemeId.Winter, (new DateTime(year, 12, 22), new DateTime(year + 1, 3, 19)));
        }

        public static ThemeDefinition? GetCurrentHolidayTheme()
        {
            var today = DateTime.Now;
            if (today.Year != HolidayRanges.First().Value.Start.Year)
            {
                InitializeHolidayRanges(today.Year);
            }

            foreach (var range in HolidayRanges)
            {
                if (today >= range.Value.Start && today < range.Value.End)
                {
                    return ThemeManager.AvailableThemes.FirstOrDefault(t => t.Id == range.Key);
                }
            }

            return null;
        }

        // Helper methods for variable date holidays
        private static DateTime GetThanksgivingDate(int year)
        {
            var nov1 = new DateTime(year, 11, 1);
            var daysUntilThursday = ((int)DayOfWeek.Thursday - (int)nov1.DayOfWeek + 7) % 7;
            return nov1.AddDays(daysUntilThursday + 21); // 4th Thursday
        }

        private static DateTime GetEasterDate(int year)
        {
            // Meeus/Jones/Butcher algorithm for Easter Sunday
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;
            return new DateTime(year, month, day);
        }

        private static DateTime GetMothersDay(int year)
        {
            var may1 = new DateTime(year, 5, 1);
            var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)may1.DayOfWeek + 7) % 7;
            return may1.AddDays(daysUntilSunday + 7); // 2nd Sunday
        }

        private static DateTime GetFathersDay(int year)
        {
            var jun1 = new DateTime(year, 6, 1);
            var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)jun1.DayOfWeek + 7) % 7;
            return jun1.AddDays(daysUntilSunday + 14); // 3rd Sunday
        }
    }
}
