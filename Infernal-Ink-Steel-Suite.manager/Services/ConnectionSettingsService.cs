using System;
using System.IO;
using System.Text.Json;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Services
{
    public sealed class ConnectionSettingsService
    {
        private readonly string _settingsPath;

        public ConnectionSettingsService()
        {
            string baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "InfernalInkSteelSuite");

            Directory.CreateDirectory(baseDir);

            _settingsPath = Path.Combine(baseDir, "connectionsettings.json");
        }

        public ConnectionSettings Load()
        {
            if (!File.Exists(_settingsPath))
            {
                var defaults = CreateDefaultSettings();
                Save(defaults);
                return defaults;
            }

            try
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonSerializer.Deserialize<ConnectionSettings>(json)
                       ?? CreateDefaultSettings();
            }
            catch
            {
                // Fallback if corrupt
                return CreateDefaultSettings();
            }
        }

        public void Save(ConnectionSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_settingsPath, json);
        }

        private static ConnectionSettings CreateDefaultSettings()
        {
            string baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "InfernalInkSteelSuite");
            string cacheDir = Path.Combine(baseDir, "cache");
            string uploadsDir = Path.Combine(baseDir, "uploads-cache");

            Directory.CreateDirectory(cacheDir);
            Directory.CreateDirectory(uploadsDir);

            return new ConnectionSettings
            {
                Mode = DataMode.LocalOnly,
                LocalDbPath = Path.Combine(cacheDir, "cache.db"),
                LocalUploadRoot = uploadsDir,
                UseHttps = true,
                SyncIntervalMinutes = 60
            };
        }
    }
}
