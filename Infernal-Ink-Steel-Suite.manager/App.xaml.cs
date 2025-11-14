using System.Windows;
using InfernalInkSteelSuite.Data;
using System;

namespace InfernalInkSteelSuite
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");
            var connectionString = $"Data Source={dbPath}";

            var databaseManager = new DatabaseManager(connectionString);
            databaseManager.InitializeDatabase();

            var login = new Login(connectionString);
            login.Show();
        }
    }
}
