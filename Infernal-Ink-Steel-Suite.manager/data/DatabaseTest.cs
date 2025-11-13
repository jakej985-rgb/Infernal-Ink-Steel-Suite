using System.Diagnostics;

namespace InfernalInkSteelSuite.Data
{
    public static class DatabaseTest
    {
        public static void TestConnection()
        {
            using var conn = Database.CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Debug.WriteLine($"Table: {reader.GetString(0)}");
            }
        }
    }
}
