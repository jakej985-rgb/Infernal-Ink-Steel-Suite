using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Client? Get(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, firstName, middleName, lastName, phone, email, notes, visits FROM clients WHERE id = $id;";
                command.Parameters.AddWithValue("$id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Client
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MiddleName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            LastName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Email = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            Notes = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Visits = reader.GetInt32(7)
                        };
                    }
                }
            }
            return null;
        }

        public List<Client> GetAll()
        {
            var result = new List<Client>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, firstName, middleName, lastName, phone, email, notes, visits FROM clients ORDER BY firstName, lastName;";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var client = new Client
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MiddleName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            LastName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Email = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            Notes = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            Visits = reader.GetInt32(7)
                        };
                        result.Add(client);
                    }
                }
            }

            return result;
        }

        public void Insert(Client client)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO clients (firstName, middleName, lastName, phone, email, notes, visits)
                      VALUES ($firstName, $middleName, $lastName, $phone, $email, $notes, $visits);";

                command.Parameters.AddWithValue("$firstName", client.FirstName);
                command.Parameters.AddWithValue("$middleName", (object)client.MiddleName ?? DBNull.Value);
                command.Parameters.AddWithValue("$lastName", client.LastName);
                command.Parameters.AddWithValue("$phone", client.Phone);
                command.Parameters.AddWithValue("$email", client.Email);
                command.Parameters.AddWithValue("$notes", client.Notes);
                command.Parameters.AddWithValue("$visits", client.Visits);

                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid();";
                client.Id = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(Client client)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"UPDATE clients
                      SET firstName = $firstName,
                          middleName = $middleName,
                          lastName = $lastName,
                          phone = $phone,
                          email = $email,
                          notes = $notes,
                          visits = $visits
                      WHERE id = $id;";

                command.Parameters.AddWithValue("$firstName", client.FirstName);
                command.Parameters.AddWithValue("$middleName", (object)client.MiddleName ?? DBNull.Value);
                command.Parameters.AddWithValue("$lastName", client.LastName);
                command.Parameters.AddWithValue("$phone", client.Phone);
                command.Parameters.AddWithValue("$email", client.Email);
                command.Parameters.AddWithValue("$notes", client.Notes);
                command.Parameters.AddWithValue("$visits", client.Visits);
                command.Parameters.AddWithValue("$id", client.Id);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM clients WHERE id = $id;";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
        }

        public string? GetClientNameById(int clientId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT firstName, middleName, lastName FROM clients WHERE id = $id";
                command.Parameters.AddWithValue("$id", clientId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var parts = new List<string>();
                        if (!reader.IsDBNull(0)) parts.Add(reader.GetString(0));
                        if (!reader.IsDBNull(1)) parts.Add(reader.GetString(1));
                        if (!reader.IsDBNull(2)) parts.Add(reader.GetString(2));
                        return string.Join(" ", parts);
                    }
                }
            }
            return null;
        }

        public int GetClientIdByName(string name)
        {
            var normalizedName = name.Trim();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id FROM clients WHERE LOWER(firstName || ' ' || middleName || ' ' || lastName) = LOWER($name) OR LOWER(firstName || ' ' || lastName) = LOWER($name)";
                command.Parameters.AddWithValue("$name", normalizedName);

                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            return 0;
        }
    }
}
