using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public class ShopSettingsRepository
    {
        public ShopSettings LoadSettings()
        {
            var settings = new ShopSettings();
            using (var connection = Database.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT key, value FROM shop_settings";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string key = reader.GetString(0);
                        string value = reader.GetString(1);
                        switch (key)
                        {
                            case "shopName":
                                settings.ShopName = value;
                                break;
                            case "loginHeadline":
                                settings.LoginHeadline = value;
                                break;
                            case "loginTagline":
                                settings.LoginTagline = value;
                                break;
                            case "accentColor":
                                settings.AccentColor = value;
                                break;
                            case "loginBackgroundPath":
                                settings.LoginBackgroundPath = value;
                                break;
                        }
                    }
                }
            }
            return settings;
        }
    }
}
