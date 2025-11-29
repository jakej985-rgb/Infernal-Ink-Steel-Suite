using System.Windows;
using InfernalInkSteelSuite.Data;
using SQLitePCL;
using System;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite
{
    public partial class App : Application
    {
        public static string ConnectionString { get; private set; } = "";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                SQLitePCL.Batteries.Init();

                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "infernalinksteelsuite.db");
                ConnectionString = $"Data Source={dbPath}";

                var databaseManager = new DatabaseManager(ConnectionString);
                databaseManager.InitializeDatabase();

                HolidayThemeService.Initialize(ConnectionString);

                ThemeManager.ApplyTheme(ThemeId.InfernalNeon);

                var holidayTheme = HolidayThemeService.GetCurrentHolidayTheme();
                if (holidayTheme != null)
                {
                    ThemeManager.ApplyTheme(holidayTheme.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }

            // Initialize font size
            ApplyFontSize();
            SettingsUpdateService.OnSettingsChanged += () => ApplyFontSize();
        }

        private static void ApplyFontSize()
        {
            try
            {
                var repo = new ShopSettingsRepository(ConnectionString);
                var settings = repo.LoadSettings();
                if (settings != null)
                {
                    Application.Current.Resources["StandardFontSize"] = settings.AppFontSize;
                }
            }
            catch { }
        }
    }
}
