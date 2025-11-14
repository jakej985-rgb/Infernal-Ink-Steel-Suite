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

        public void CreateTable()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"CREATE TABLE IF NOT EXISTS shopsettings (
                        shopName TEXT,
                        logoPath TEXT,
                        accentColor TEXT,
                        sidebarArtworkPath TEXT,
                        loginHeadline TEXT,
                        loginTagline TEXT,
                        loginBackgroundPath TEXT,
                        loginHeadlineFont TEXT,
                        loginTaglineFont TEXT,
                        loginTextColor TEXT,
                        tattooPerHour REAL,
                        piercingSingle REAL,
                        piercingMulti REAL)";
                command.ExecuteNonQuery();

                var migrations = new[]
                {
                    "ALTER TABLE shopsettings ADD COLUMN loginHeadline TEXT",
                    "ALTER TABLE shopsettings ADD COLUMN loginTagline TEXT",
                    "ALTER TABLE shopsettings ADD COLUMN loginBackgroundPath TEXT",
                    "ALTER TABLE shopsettings ADD COLUMN loginHeadlineFont TEXT",
                    "ALTER TABLE shopsettings ADD COLUMN loginTaglineFont TEXT",
                    "ALTER TABLE shopsettings ADD COLUMN loginTextColor TEXT"
                };

                foreach (var migration in migrations)
                {
                    try
                    {
                        command.CommandText = migration;
                        command.ExecuteNonQuery();
                    }
                    catch (SqliteException ex) when (ex.Message.Contains("duplicate column"))
                    {
                        // Ignore duplicate column errors during migration
                    }
                }
            }
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
                        @"INSERT INTO shopsettings (shopName, logoPath, accentColor, sidebarArtworkPath, loginHeadline, loginTagline, loginBackgroundPath, loginHeadlineFont, loginTaglineFont, loginTextColor, tattooPerHour, piercingSingle, piercingMulti)
                        VALUES ($shopName, $logoPath, $accentColor, $sidebarArtworkPath, $loginHeadline, $loginTagline, $loginBackgroundPath, $loginHeadlineFont, $loginTaglineFont, $loginTextColor, $tattooPerHour, $piercingSingle, $piercingMulti)";

                    command.Parameters.AddWithValue("$shopName", settings.ShopName);
                    command.Parameters.AddWithValue("$logoPath", settings.LogoPath);
                    command.Parameters.AddWithValue("$accentColor", settings.AccentColor);
                    command.Parameters.AddWithValue("$sidebarArtworkPath", settings.SidebarArtworkPath);
                    command.Parameters.AddWithValue("$loginHeadline", settings.LoginHeadline);
                    command.Parameters.AddWithValue("$loginTagline", settings.LoginTagline);
                    command.Parameters.AddWithValue("$loginBackgroundPath", settings.LoginBackgroundPath);
                    command.Parameters.AddWithValue("$loginHeadlineFont", settings.LoginHeadlineFontFamily);
                    command.Parameters.AddWithValue("$loginTaglineFont", settings.LoginTaglineFontFamily);
                    command.Parameters.AddWithValue("$loginTextColor", settings.LoginTextColor);
                    command.Parameters.AddWithValue("$tattooPerHour", settings.TattooPerHour);
                    command.Parameters.AddWithValue("$piercingSingle", settings.PiercingSingle);
                    command.Parameters.AddWithValue("$piercingMulti", settings.PiercingMulti);

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
                command.CommandText = "SELECT * FROM shopsettings LIMIT 1";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        settings.ShopName = reader["shopName"].ToString();
                        settings.LogoPath = reader["logoPath"].ToString();
                        settings.AccentColor = reader["accentColor"].ToString();
                        settings.SidebarArtworkPath = reader["sidebarArtworkPath"].ToString();
                        settings.LoginHeadline = reader["loginHeadline"].ToString();
                        settings.LoginTagline = reader["loginTagline"].ToString();
                        settings.LoginBackgroundPath = reader["loginBackgroundPath"].ToString();
                        settings.LoginHeadlineFontFamily = reader["loginHeadlineFont"].ToString();
                        settings.LoginTaglineFontFamily = reader["loginTaglineFont"].ToString();
                        settings.LoginTextColor = reader["loginTextColor"].ToString();
                        settings.TattooPerHour = Convert.ToDouble(reader["tattooPerHour"]);
                        settings.PiercingSingle = Convert.ToDouble(reader["piercingSingle"]);
                        settings.PiercingMulti = Convert.ToDouble(reader["piercingMulti"]);
                    }
                }
            }
            return settings;
        }
    }
}
