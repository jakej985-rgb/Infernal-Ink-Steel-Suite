using System.Windows;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.DataProviders; // For DataProviderFactory
using SQLitePCL;
using System;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Services.Sync;
using Refit;
using InfernalInkSteelSuite.Repositories; // Might be needed for DatabaseManager or others
using InfernalInkSteelSuite.Domain; // For IDataProvider, DataMode

namespace InfernalInkSteelSuite
{
    public partial class App : Application
    {
        // Removed static ConnectionString property as it promotes hidden dependencies. 
        // If needed for temporary compatibility, we could keep it but it should be avoided.

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                SQLitePCL.Batteries.Init();

                // 1. Load Connection Settings
                var settingsService = new ConnectionSettingsService();
                var connectionSettings = settingsService.Load();

                // 2. Create Data Provider
                var dataProvider = DataProviderFactory.Create(connectionSettings);

                // Initialize Database if Local (this logic was previously in OnStartup)
                // We might want to move this to DataProvider or a DatabaseInitializer, but for now:
                // Initialize Database if Local or Offline Cache (both use local DB)
                if (connectionSettings.Mode == InfernalInkSteelSuite.Domain.DataMode.LocalOnly ||
                    connectionSettings.Mode == InfernalInkSteelSuite.Domain.DataMode.ServerWithOfflineCache)
                {
                    var dbDir = System.IO.Path.GetDirectoryName(connectionSettings.LocalDbPath);
                    if (!string.IsNullOrEmpty(dbDir) && !System.IO.Directory.Exists(dbDir))
                    {
                        System.IO.Directory.CreateDirectory(dbDir);
                    }
                    // Ideally DataProvider does this, but for v1 we keep existing logic:
                    var databaseManager = new DatabaseManager($"Data Source={connectionSettings.LocalDbPath}");
                    databaseManager.InitializeDatabase();

                    HolidayThemeService.Initialize($"Data Source={connectionSettings.LocalDbPath}");
                }

                if (connectionSettings.Mode == InfernalInkSteelSuite.Domain.DataMode.ServerWithOfflineCache &&
                    dataProvider is SyncingDataProvider syncingProvider)
                {
                    // Initialize Sync Engine
                    var apiClient = Refit.RestService.For<IApiClient>(connectionSettings.ServerBaseUrl ?? "http://localhost:5000");
                    var syncEngine = new SyncEngine(syncingProvider.SyncQueue, apiClient, syncingProvider.LocalProvider);
                    syncEngine.Start();
                    Properties["SyncEngine"] = syncEngine;
                }

                // 3. Theme Setup
                ThemeManager.ApplyTheme(ThemeId.InfernalNeon);
                var holidayTheme = HolidayThemeService.GetCurrentHolidayTheme();
                if (holidayTheme != null)
                {
                    ThemeManager.ApplyTheme(holidayTheme.Id);
                }

                // 4. Show Login
                var loginWindow = new Login(dataProvider);
                loginWindow.Show();

                // 5. Apply Font Size (Requires separate handling or passed through provider)
                ApplyFontSize(dataProvider);
                SettingsUpdateService.OnSettingsChanged += () => ApplyFontSize(dataProvider);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private static void ApplyFontSize(InfernalInkSteelSuite.Domain.IDataProvider dataProvider)
        {
            try
            {
                var settings = dataProvider.ShopSettings.LoadSettings();
                if (settings != null)
                {
                    Application.Current.Resources["StandardFontSize"] = settings.AppFontSize;
                }
            }
            catch { }
        }
    }
}
