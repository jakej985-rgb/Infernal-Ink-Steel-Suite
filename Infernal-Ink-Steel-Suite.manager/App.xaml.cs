using System.Windows;
using InfernalInkSteelSuite.Data;
using SQLitePCL;
using System;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InfernalInkSteelSuite
{
    public partial class App : Application
    {
        public static string ConnectionString { get; private set; } = "";
        public static AppDbContext? LocalDb { get; private set; }
        public static BackgroundSyncService? SyncService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                SQLitePCL.Batteries.Init();

                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var dbDir = System.IO.Path.Combine(appData, "InfernalInkSteelSuite", "Data");
                if (!System.IO.Directory.Exists(dbDir))
                {
                    System.IO.Directory.CreateDirectory(dbDir);
                }
                var dbPath = System.IO.Path.Combine(dbDir, "infernalinksteel.db");
                ConnectionString = $"Data Source={dbPath}";

                // Initialize EF Core Context for Desktop (SQLite)
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlite(ConnectionString);
                LocalDb = new AppDbContext(optionsBuilder.Options);

                // Add retry logic for database creation to handle race conditions with API
                int retryCount = 0;
                bool created = false;
                while (!created && retryCount < 5)
                {
                    try
                    {
                        using var mutex = new System.Threading.Mutex(false, "Global\\InfernalInkSteelSuiteDbMigration");
                        var hasHandle = false;
                        try
                        {
                            hasHandle = mutex.WaitOne(TimeSpan.FromSeconds(30), false);
                            if (!hasHandle) throw new TimeoutException("Timeout waiting for exclusive access to DB migration.");
                            LocalDb.Database.Migrate();
                        }
                        finally
                        {
                            if (hasHandle) mutex.ReleaseMutex();
                        }
                        created = true;

                        // Seed default admin if no users exist
                        if (!LocalDb.Users.Any())
                        {
                            var hasher = new InfernalInkSteelSuite.Repositories.Services.PasswordHasher();
                            var userRepo = new UserRepository(LocalDb, hasher);
                            userRepo.AddUser("Admin", "admin123", "Admin");
                        }
                    }
                    catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 5) // SQLITE_BUSY
                    {
                        retryCount++;
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                // Initialize Sync Services
                var syncClient = new SyncClient();
                SyncService = new BackgroundSyncService(ConnectionString, syncClient);
                SyncService.Start();

                if (LocalDb != null)
                {
                    HolidayThemeService.Initialize(LocalDb as InfernalInkSteelSuite.Data.AppDbContext);
                }

                ThemeManager.ApplyTheme(ThemeId.InfernalNeon);

                var holidayTheme = HolidayThemeService.GetCurrentHolidayTheme();
                if (holidayTheme != null)
                {
                    ThemeManager.ApplyHolidayTheme(holidayTheme.Id);
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
                if (LocalDb == null) return;
                var repo = new ShopSettingsRepository(LocalDb);
                var settings = repo.LoadSettings();
                if (settings != null)
                {
                    Application.Current.Resources["StandardFontSize"] = settings.AppFontSize;
                }
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SyncService?.Stop();
            base.OnExit(e);
        }
    }
}
