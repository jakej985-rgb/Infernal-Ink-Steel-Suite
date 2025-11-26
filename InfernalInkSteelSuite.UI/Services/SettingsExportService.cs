using InfernalInkSteelSuite.Domain;
using System;
using System.IO;
using System.Text.Json;

namespace InfernalInkSteelSuite.UI.Services
{
    public class SettingsExportService
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public static bool ExportSettings(ShopSettings settings, string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, _options);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ShopSettings? ImportSettings(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                var json = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<ShopSettings>(json);
                return settings;
            }
            catch
            {
                return null;
            }
        }

        public static bool ValidateSettings(ShopSettings settings)
        {
            if (settings == null) return false;

            // Basic validation
            if (settings.TaxRate < 0 || settings.TaxRate > 100) return false;
            if (settings.DepositAmount < 0) return false;
            if (settings.BookingBufferMinutes < 0) return false;

            return true;
        }
    }
}
