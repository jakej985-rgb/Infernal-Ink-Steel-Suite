using Microsoft.Data.Sqlite;

namespace InfernalInkSteelSuite.Data
{
    public static class Database
    {
        private const string ConnectionString = "Data Source=shop_manager.db";

        public static SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
