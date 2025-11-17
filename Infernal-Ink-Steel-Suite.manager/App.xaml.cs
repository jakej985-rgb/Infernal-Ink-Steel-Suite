using System.Windows;
using InfernalInkSteelSuite.Data;
using SQLitePCL;
using System;
using System.Windows.Media;
using InfernalInkSteelSuite.Services;

namespace InfernalInkSteelSuite
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                SQLitePCL.Batteries.Init();

                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");
                var connectionString = $"Data Source={dbPath}";

                var databaseManager = new DatabaseManager(connectionString);
                databaseManager.InitializeDatabase();

                ThemeManager.Instance.ThemeChanged += OnThemeChanged;
                ApplyTheme(ThemeManager.Instance.CurrentTheme);

                var login = new Login(connectionString);
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void OnThemeChanged(Theme theme)
        {
            ApplyTheme(theme);
        }

        private void ApplyTheme(Theme theme)
        {
            Resources["PrimaryColor"] = theme.PrimaryColor;
            Resources["SecondaryColor"] = theme.SecondaryColor;
        }
    }
}
