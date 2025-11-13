using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public class ClientRepository
    {
        public List<Client> GetAll()
        {
            var result = new List<Client>();

            using var conn = Database.CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, phone, email, notes, visits FROM clients ORDER BY name;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var client = new Client
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Phone = reader.GetString(2),
                    Email = reader.GetString(3),
                    Notes = reader.GetString(4),
                    Visits = reader.GetInt32(5)
                };
                result.Add(client);
            }

            return result;
        }

        public void Add(Client client)
        {
            using var conn = Database.CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"INSERT INTO clients (name, phone, email, notes, visits)
                  VALUES ($name, $phone, $email, $notes, $visits);";

            cmd.Parameters.AddWithValue("$name", client.Name);
            cmd.Parameters.AddWithValue("$phone", client.Phone);
            cmd.Parameters.AddWithValue("$email", client.Email);
            cmd.Parameters.AddWithValue("$notes", client.Notes);
            cmd.Parameters.AddWithValue("$visits", client.Visits);

            cmd.ExecuteNonQuery();
        }
    }
}
