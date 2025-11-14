using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace InfernalInkSteelSuite.Data
{
    public static class Database
    {
        private const string DbFileName = "shop_manager.db";

        public static string ConnectionString
        {
            get
            {
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);
                return new SqliteConnectionStringBuilder
                {
                    DataSource = dbPath
                }.ToString();
            }
        }

        public static SqliteConnection CreateConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
