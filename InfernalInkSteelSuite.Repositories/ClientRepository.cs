using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public class ClientRepository
    {
        public Client Get(int id)
        {
            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, firstName, middleName, lastName, phone, email, notes, visits FROM clients WHERE id = $id;";
                cmd.Parameters.AddWithValue("$id", id);

                using (var reader = cmd.ExecuteReader())
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

            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, firstName, middleName, lastName, phone, email, notes, visits FROM clients ORDER BY firstName, lastName;";

                using (var reader = cmd.ExecuteReader())
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
            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO clients (firstName, middleName, lastName, phone, email, notes, visits)
                      VALUES ($firstName, $middleName, $lastName, $phone, $email, $notes, $visits);";

                cmd.Parameters.AddWithValue("$firstName", client.FirstName);
                cmd.Parameters.AddWithValue("$middleName", (object)client.MiddleName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$lastName", client.LastName);
                cmd.Parameters.AddWithValue("$phone", client.Phone);
                cmd.Parameters.AddWithValue("$email", client.Email);
                cmd.Parameters.AddWithValue("$notes", client.Notes);
                cmd.Parameters.AddWithValue("$visits", client.Visits);

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid();";
                client.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Update(Client client)
        {
            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    @"UPDATE clients
                      SET firstName = $firstName,
                          middleName = $middleName,
                          lastName = $lastName,
                          phone = $phone,
                          email = $email,
                          notes = $notes,
                          visits = $visits
                      WHERE id = $id;";

                cmd.Parameters.AddWithValue("$firstName", client.FirstName);
                cmd.Parameters.AddWithValue("$middleName", (object)client.MiddleName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$lastName", client.LastName);
                cmd.Parameters.AddWithValue("$phone", client.Phone);
                cmd.Parameters.AddWithValue("$email", client.Email);
                cmd.Parameters.AddWithValue("$notes", client.Notes);
                cmd.Parameters.AddWithValue("$visits", client.Visits);
                cmd.Parameters.AddWithValue("$id", client.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM clients WHERE id = $id;";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
