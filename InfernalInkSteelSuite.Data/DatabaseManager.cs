using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace InfernalInkSteelSuite.Data
{
    public class DatabaseManager
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                File.Create(DbPath).Close();
            }

            using (var connection = new SqliteConnection($"Data Source={DbPath}"))
            {
                connection.Open();

                var appointmentsCommand = connection.CreateCommand();
                appointmentsCommand.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS appointments (
                        id INTEGER PRIMARY KEY,
                        clientId INTEGER,
                        userId INTEGER,
                        dateTime TEXT,
                        durationMinutes INTEGER,
                        serviceType TEXT,
                        serviceCategory TEXT,
                        priceType TEXT,
                        priceCharged REAL,
                        notes TEXT,
                        clientName TEXT,
                        color TEXT,
                        status TEXT
                    )
                ";
                appointmentsCommand.ExecuteNonQuery();

                var clientsCommand = connection.CreateCommand();
                clientsCommand.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS clients (
                        id INTEGER PRIMARY KEY,
                        firstName TEXT,
                        middleName TEXT,
                        lastName TEXT,
                        phone TEXT,
                        email TEXT
                    )
                ";
                clientsCommand.ExecuteNonQuery();

                var documentsCommand = connection.CreateCommand();
                documentsCommand.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS documents (
                        id INTEGER PRIMARY KEY,
                        userId INTEGER,
                        clientId INTEGER,
                        title TEXT,
                        filePath TEXT,
                        createdAt TEXT
                    )
                ";
                documentsCommand.ExecuteNonQuery();

                var usersCommand = connection.CreateCommand();
                usersCommand.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS users (
                        id INTEGER PRIMARY KEY,
                        username TEXT,
                        password TEXT,
                        role TEXT
                    )
                ";
                usersCommand.ExecuteNonQuery();
            }
        }
    }
}
