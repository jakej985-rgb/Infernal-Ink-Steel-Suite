using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace InfernalInkSteelSuite.Repositories
{
    public class ClientRepository
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbPath}");
        }

        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM clients";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clients.Add(new Client
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            MiddleName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            Phone = reader.GetString(4),
                            Email = reader.GetString(5)
                        });
                    }
                }
            }
            return clients;
        }

        public Client GetClientById(int id)
        {
            Client client = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM clients WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        client = new Client
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            MiddleName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            Phone = reader.GetString(4),
                            Email = reader.GetString(5)
                        };
                    }
                }
            }
            return client;
        }

        public void AddClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO clients (firstName, middleName, lastName, phone, email)
                    VALUES (@firstName, @middleName, @lastName, @phone, @email)
                ";
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@middleName", client.MiddleName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@phone", client.Phone);
                command.Parameters.AddWithValue("@email", client.Email);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateClient(Client client)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE clients
                    SET firstName = @firstName, middleName = @middleName, lastName = @lastName, phone = @phone, email = @email
                    WHERE id = @id
                ";
                command.Parameters.AddWithValue("@id", client.Id);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@middleName", client.MiddleName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@phone", client.Phone);
                command.Parameters.AddWithValue("@email", client.Email);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteClient(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM clients WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
