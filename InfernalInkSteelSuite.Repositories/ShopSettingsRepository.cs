using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Domain;
using System;

namespace InfernalInkSteelSuite.Repositories
{
    public class ShopSettingsRepository : IShopSettingsRepository
    {
        private readonly string _connectionString;

        public ShopSettingsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveSettings(ShopSettings settings)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM shopsettings";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        @"INSERT INTO shopsettings (shopName, logoPath, accentColor, sidebarArtworkPath, loginHeadline, loginTagline, loginBackgroundPath, loginHeadlineFontFamily, loginTaglineFontFamily, loginTextColor, tattooPerHour, piercingSingle, piercingMulti, shopMinimumRate, EnableAutomaticHolidayThemes)
                        VALUES ($shopName, $logoPath, $accentColor, $sidebarArtworkPath, $loginHeadline, $loginTagline, $loginBackgroundPath, $loginHeadlineFontFamily, $loginTaglineFontFamily, $loginTextColor, $tattooPerHour, $piercingSingle, $piercingMulti, $shopMinimumRate, $EnableAutomaticHolidayThemes)";

                    command.Parameters.AddWithValue("$shopName", settings.ShopName);
                    command.Parameters.AddWithValue("$logoPath", settings.LogoPath);
                    command.Parameters.AddWithValue("$accentColor", settings.AccentColor);
                    command.Parameters.AddWithValue("$sidebarArtworkPath", settings.SidebarArtworkPath);
                    command.Parameters.AddWithValue("$loginHeadline", settings.LoginHeadline);
                    command.Parameters.AddWithValue("$loginTagline", settings.LoginTagline);
                    command.Parameters.AddWithValue("$loginBackgroundPath", settings.LoginBackgroundPath);
                    command.Parameters.AddWithValue("$loginHeadlineFontFamily", settings.LoginHeadlineFontFamily);
                    command.Parameters.AddWithValue("$loginTaglineFontFamily", settings.LoginTaglineFontFamily);
                    command.Parameters.AddWithValue("$loginTextColor", settings.LoginTextColor);
                    command.Parameters.AddWithValue("$tattooPerHour", settings.TattooPerHour);
                    command.Parameters.AddWithValue("$piercingSingle", settings.PiercingSingle);
                    command.Parameters.AddWithValue("$piercingMulti", settings.PiercingMulti);
                    command.Parameters.AddWithValue("$shopMinimumRate", settings.ShopMinimumRate);
                    command.Parameters.AddWithValue("$EnableAutomaticHolidayThemes", settings.EnableAutomaticHolidayThemes ? 1 : 0);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        }

        public ShopSettings LoadSettings()
        {
            var settings = new ShopSettings();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT shopName, logoPath, accentColor, sidebarArtworkPath, loginHeadline, loginTagline, loginBackgroundPath, loginHeadlineFontFamily, loginTaglineFontFamily, loginTextColor, tattooPerHour, piercingSingle, piercingMulti, shopMinimumRate, EnableAutomaticHolidayThemes FROM shopsettings LIMIT 1";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        settings.ShopName = reader["shopName"].ToString() ?? string.Empty;
                        settings.LogoPath = reader["logoPath"].ToString() ?? string.Empty;
                        settings.AccentColor = reader["accentColor"].ToString() ?? string.Empty;
                        settings.SidebarArtworkPath = reader["sidebarArtworkPath"].ToString() ?? string.Empty;
                        settings.LoginHeadline = reader["loginHeadline"].ToString() ?? string.Empty;
                        settings.LoginTagline = reader["loginTagline"].ToString() ?? string.Empty;
                        settings.LoginBackgroundPath = reader["loginBackgroundPath"].ToString() ?? string.Empty;
                        settings.LoginHeadlineFontFamily = reader["loginHeadlineFontFamily"].ToString() ?? string.Empty;
                        settings.LoginTaglineFontFamily = reader["loginTaglineFontFamily"].ToString() ?? string.Empty;
                        settings.LoginTextColor = reader["loginTextColor"].ToString() ?? string.Empty;
                        settings.TattooPerHour = Convert.ToDouble(reader["tattooPerHour"]);
                        settings.PiercingSingle = Convert.ToDouble(reader["piercingSingle"]);
                        settings.PiercingMulti = Convert.ToDouble(reader["piercingMulti"]);
                        settings.ShopMinimumRate = Convert.ToDouble(reader["shopMinimumRate"]);
                        settings.EnableAutomaticHolidayThemes = Convert.ToInt32(reader["EnableAutomaticHolidayThemes"]) == 1;
                    }
                }
            }
            return settings;
        }
    }
}
