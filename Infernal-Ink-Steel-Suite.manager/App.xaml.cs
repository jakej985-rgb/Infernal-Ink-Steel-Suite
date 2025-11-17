using System.Windows;
using InfernalInkSteelSuite.Data;
using SQLitePCL;
using System;
using InfernalInkSteelSuite.Services;

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

                // Default theme before login (e.g. Infernal Neon)
                ThemeManager.ApplyTheme(ThemeId.InfernalNeon);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
