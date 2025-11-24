using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Domain;
using System;

namespace InfernalInkSteelSuite.Repositories
{
    public class ShopSettingsRepository(string connectionString) : IShopSettingsRepository
    {
        private readonly string _connectionString = connectionString;

        public void SaveSettings(ShopSettings settings)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Ensure all columns exist
            EnsureColumnExists(connection, "ShopHoursJson", "TEXT");
            EnsureColumnExists(connection, "TaxRate", "REAL");
            EnsureColumnExists(connection, "DepositType", "TEXT");
            EnsureColumnExists(connection, "DepositAmount", "REAL");
            EnsureColumnExists(connection, "BookingBufferMinutes", "INTEGER");
            EnsureColumnExists(connection, "CancellationPolicy", "TEXT");
            EnsureColumnExists(connection, "AppointmentDurationPresetsJson", "TEXT");
            EnsureColumnExists(connection, "SpecialHoursJson", "TEXT");
            EnsureColumnExists(connection, "NotificationSettingsJson", "TEXT");
            EnsureColumnExists(connection, "BackupSettingsJson", "TEXT");

            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM shopsettings";
            command.ExecuteNonQuery();

            command.CommandText =
                @"INSERT INTO shopsettings (shopName, logoPath, accentColor, sidebarArtworkPath, loginBackgroundPath, loginHeadlineFontFamily, loginTaglineFontFamily, loginTextColor, tattooPerHour, piercingSingle, piercingMulti, shopMinimumRate, EnableAutomaticHolidayThemes, IsSpecialMessageEnabled, SpecialMessageText, ShopHoursJson, TaxRate, DepositType, DepositAmount, BookingBufferMinutes, CancellationPolicy, AppointmentDurationPresetsJson, SpecialHoursJson, NotificationSettingsJson, BackupSettingsJson)
                        VALUES ($shopName, $logoPath, $accentColor, $sidebarArtworkPath, $loginBackgroundPath, $loginHeadlineFontFamily, $loginTaglineFontFamily, $loginTextColor, $tattooPerHour, $piercingSingle, $piercingMulti, $shopMinimumRate, $EnableAutomaticHolidayThemes, $IsSpecialMessageEnabled, $SpecialMessageText, $ShopHoursJson, $TaxRate, $DepositType, $DepositAmount, $BookingBufferMinutes, $CancellationPolicy, $AppointmentDurationPresetsJson, $SpecialHoursJson, $NotificationSettingsJson, $BackupSettingsJson)";

            command.Parameters.AddWithValue("$shopName", settings.ShopName);
            command.Parameters.AddWithValue("$logoPath", settings.LogoPath);
            command.Parameters.AddWithValue("$accentColor", settings.AccentColor);
            command.Parameters.AddWithValue("$sidebarArtworkPath", settings.SidebarArtworkPath);
            command.Parameters.AddWithValue("$loginBackgroundPath", settings.LoginBackgroundPath);
            command.Parameters.AddWithValue("$IsSpecialMessageEnabled", settings.IsSpecialMessageEnabled ? 1 : 0);
            command.Parameters.AddWithValue("$SpecialMessageText", settings.SpecialMessageText);
            command.Parameters.AddWithValue("$loginHeadlineFontFamily", settings.LoginHeadlineFontFamily);
            command.Parameters.AddWithValue("$loginTaglineFontFamily", settings.LoginTaglineFontFamily);
            command.Parameters.AddWithValue("$loginTextColor", settings.LoginTextColor);
            command.Parameters.AddWithValue("$tattooPerHour", settings.TattooPerHour);
            command.Parameters.AddWithValue("$piercingSingle", settings.PiercingSingle);
            command.Parameters.AddWithValue("$piercingMulti", settings.PiercingMulti);
            command.Parameters.AddWithValue("$shopMinimumRate", settings.ShopMinimumRate);
            command.Parameters.AddWithValue("$EnableAutomaticHolidayThemes", settings.EnableAutomaticHolidayThemes ? 1 : 0);
            command.Parameters.AddWithValue("$ShopHoursJson", settings.ShopHoursJson ?? string.Empty);
            command.Parameters.AddWithValue("$TaxRate", settings.TaxRate);
            command.Parameters.AddWithValue("$DepositType", settings.DepositType ?? "Percentage");
            command.Parameters.AddWithValue("$DepositAmount", settings.DepositAmount);
            command.Parameters.AddWithValue("$BookingBufferMinutes", settings.BookingBufferMinutes);
            command.Parameters.AddWithValue("$CancellationPolicy", settings.CancellationPolicy ?? string.Empty);
            command.Parameters.AddWithValue("$AppointmentDurationPresetsJson", settings.AppointmentDurationPresetsJson ?? string.Empty);
            command.Parameters.AddWithValue("$SpecialHoursJson", settings.SpecialHoursJson ?? string.Empty);
            command.Parameters.AddWithValue("$NotificationSettingsJson", settings.NotificationSettingsJson ?? string.Empty);
            command.Parameters.AddWithValue("$BackupSettingsJson", settings.BackupSettingsJson ?? string.Empty);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public ShopSettings LoadSettings()
        {
            var settings = new ShopSettings();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Ensure all columns exist
            EnsureColumnExists(connection, "ShopHoursJson", "TEXT");
            EnsureColumnExists(connection, "TaxRate", "REAL");
            EnsureColumnExists(connection, "DepositType", "TEXT");
            EnsureColumnExists(connection, "DepositAmount", "REAL");
            EnsureColumnExists(connection, "BookingBufferMinutes", "INTEGER");
            EnsureColumnExists(connection, "CancellationPolicy", "TEXT");
            EnsureColumnExists(connection, "AppointmentDurationPresetsJson", "TEXT");
            EnsureColumnExists(connection, "SpecialHoursJson", "TEXT");
            EnsureColumnExists(connection, "NotificationSettingsJson", "TEXT");
            EnsureColumnExists(connection, "BackupSettingsJson", "TEXT");
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT shopName, logoPath, accentColor, sidebarArtworkPath, loginHeadline, loginTagline, loginBackgroundPath, loginHeadlineFontFamily, loginTaglineFontFamily, loginTextColor, tattooPerHour, piercingSingle, piercingMulti, shopMinimumRate, EnableAutomaticHolidayThemes, SpecialMessageText, IsSpecialMessageEnabled, ShopHoursJson, TaxRate, DepositType, DepositAmount, BookingBufferMinutes, CancellationPolicy, AppointmentDurationPresetsJson, SpecialHoursJson, NotificationSettingsJson, BackupSettingsJson FROM shopsettings LIMIT 1";
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                settings.ShopName = reader["shopName"].ToString() ?? string.Empty;
                settings.LogoPath = reader["logoPath"].ToString() ?? string.Empty;
                settings.AccentColor = reader["accentColor"].ToString() ?? string.Empty;
                settings.SidebarArtworkPath = reader["sidebarArtworkPath"].ToString() ?? string.Empty;
                settings.SpecialMessageText = reader["SpecialMessageText"].ToString() ?? string.Empty;
                settings.LoginBackgroundPath = reader["loginBackgroundPath"].ToString() ?? string.Empty;
                settings.LoginHeadlineFontFamily = reader["loginHeadlineFontFamily"].ToString() ?? string.Empty;
                settings.IsSpecialMessageEnabled = Convert.ToInt32(reader["IsSpecialMessageEnabled"]) == 1;
                settings.LoginTaglineFontFamily = reader["loginTaglineFontFamily"].ToString() ?? string.Empty;
                settings.LoginTextColor = reader["loginTextColor"].ToString() ?? string.Empty;
                settings.TattooPerHour = Convert.ToDouble(reader["tattooPerHour"]);
                settings.PiercingSingle = Convert.ToDouble(reader["piercingSingle"]);
                settings.PiercingMulti = Convert.ToDouble(reader["piercingMulti"]);
                settings.ShopMinimumRate = Convert.ToDouble(reader["shopMinimumRate"]);
                settings.EnableAutomaticHolidayThemes = Convert.ToInt32(reader["EnableAutomaticHolidayThemes"]) == 1;
                settings.ShopHoursJson = reader["ShopHoursJson"].ToString() ?? string.Empty;

                // New settings
                settings.TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDouble(reader["TaxRate"]) : 0.0;
                settings.DepositType = reader["DepositType"]?.ToString() ?? "Percentage";
                settings.DepositAmount = reader["DepositAmount"] != DBNull.Value ? Convert.ToDouble(reader["DepositAmount"]) : 0.0;
                settings.BookingBufferMinutes = reader["BookingBufferMinutes"] != DBNull.Value ? Convert.ToInt32(reader["BookingBufferMinutes"]) : 0;
                settings.CancellationPolicy = reader["CancellationPolicy"]?.ToString() ?? string.Empty;
                settings.AppointmentDurationPresetsJson = reader["AppointmentDurationPresetsJson"]?.ToString() ?? string.Empty;
                settings.SpecialHoursJson = reader["SpecialHoursJson"]?.ToString() ?? string.Empty;
                settings.NotificationSettingsJson = reader["NotificationSettingsJson"]?.ToString() ?? string.Empty;
                settings.BackupSettingsJson = reader["BackupSettingsJson"]?.ToString() ?? string.Empty;
            }
            return settings;
        }

        private static void EnsureColumnExists(SqliteConnection connection, string columnName, string columnType)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(shopsettings)";
            bool exists = false;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"]?.ToString()?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
            {
                var alter = connection.CreateCommand();
                alter.CommandText = $"ALTER TABLE shopsettings ADD COLUMN {columnName} {columnType} DEFAULT ''";
                alter.ExecuteNonQuery();
            }
        }
    }
}
