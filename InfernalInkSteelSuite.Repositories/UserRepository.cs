using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;

namespace InfernalInkSteelSuite.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserById(int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE id = @id";
                command.Parameters.AddWithValue("@id", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Role = reader.GetString(3),
                            AvatarPath = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5),
                            UpdatedAt = reader.GetDateTime(6)
                        };
                    }
                }
            }
            return null;
        }

        public bool AddUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO users (username, passwordHash, role, avatarPath, createdAt, updatedAt)
                    VALUES (@username, @passwordHash, @role, @avatarPath, @createdAt, @updatedAt)";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@avatarPath", user.AvatarPath);
                command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                command.Parameters.AddWithValue("@updatedAt", user.UpdatedAt);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE users
                    SET username = @username,
                        passwordHash = @passwordHash,
                        role = @role,
                        avatarPath = @avatarPath,
                        updatedAt = @updatedAt
                    WHERE id = @id";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@avatarPath", user.AvatarPath);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", user.Id);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM users WHERE id = @id";
                command.Parameters.AddWithValue("@id", userId);

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
