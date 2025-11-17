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

                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");
                ConnectionString = $"Data Source={dbPath}";

                var databaseManager = new DatabaseManager(ConnectionString);
                databaseManager.InitializeDatabase();

                var shopSettingsRepository = new ShopSettingsRepository(ConnectionString);
                var settings = shopSettingsRepository.LoadSettings();

                // Default theme before login (e.g. Infernal Neon)
                ThemeManager.ApplyTheme(ThemeId.InfernalNeon);

                if (settings.EnableAutomaticHolidayThemes)
                {
                    var holidayTheme = HolidayThemeService.GetCurrentHolidayTheme();
                    if (holidayTheme != null)
                    {
                        ThemeManager.ApplyTheme(holidayTheme.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
