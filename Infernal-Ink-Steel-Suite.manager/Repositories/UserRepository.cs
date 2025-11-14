using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public class UserRepository
    {
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = Database.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, username, password_hash, role FROM users";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Role = reader.GetString(3)
                        });
                    }
                }
            }
            return users;
        }

        public User GetUserByUsername(string username)
        {
            User user = null;
            using (var connection = Database.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, username, password_hash, role FROM users WHERE username = $username";
                command.Parameters.AddWithValue("$username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Role = reader.GetString(3)
                        };
                    }
                }
            }
            return user;
        }
    }
}
