using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public class DocumentRepository(string connectionString) : IDocumentRepository
    {
        private readonly string _connectionString = connectionString;

        public Document? Get(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, userId, clientId, title, filePath, createdAt FROM documents WHERE id = $id";
                command.Parameters.AddWithValue("$id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Document
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ClientId = reader.GetInt32(2),
                            Title = reader.GetString(3),
                            FilePath = reader.GetString(4),
                            CreatedAt = DateTime.Parse(reader.GetString(5))
                        };
                    }
                }
            }
            return null;
        }

        public List<Document> GetAll()
        {
            var result = new List<Document>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, userId, clientId, title, filePath, createdAt FROM documents ORDER BY createdAt DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Document
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ClientId = reader.GetInt32(2),
                            Title = reader.GetString(3),
                            FilePath = reader.GetString(4),
                            CreatedAt = DateTime.Parse(reader.GetString(5))
                        });
                    }
                }
            }
            return result;
        }

        public List<Document> GetDocuments(int userId, string role, int maxDocuments, out bool truncated)
        {
            var result = new List<Document>();
            truncated = false;

            if (maxDocuments <= 0)
            {
                return result;
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                if (role == "Admin" || role == "Manager")
                {
                    command.CommandText = "SELECT * FROM documents ORDER BY createdAt DESC";
                }
                else
                {
                    command.CommandText = "SELECT * FROM documents WHERE userId = $userId ORDER BY createdAt DESC";
                    command.Parameters.AddWithValue("$userId", userId);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (result.Count >= maxDocuments)
                        {
                            truncated = true;
                            break;
                        }
                        result.Add(new Document
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ClientId = reader.GetInt32(2),
                            Title = reader.GetString(3),
                            FilePath = reader.GetString(4),
                            CreatedAt = DateTime.Parse(reader.GetString(5))
                        });
                    }
                }
            }
            return result;
        }

        public void Insert(Document document)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO documents (userId, clientId, title, filePath, createdAt)
                      VALUES ($userId, $clientId, $title, $filePath, $createdAt);";

                command.Parameters.AddWithValue("$userId", document.UserId);
                command.Parameters.AddWithValue("$clientId", document.ClientId);
                command.Parameters.AddWithValue("$title", document.Title);
                command.Parameters.AddWithValue("$filePath", document.FilePath);
                command.Parameters.AddWithValue("$createdAt", document.CreatedAt.ToString("o"));

                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid();";
                document.Id = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(Document document)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"UPDATE documents
                      SET userId = $userId,
                          clientId = $clientId,
                          title = $title,
                          filePath = $filePath,
                          createdAt = $createdAt
                      WHERE id = $id;";

                command.Parameters.AddWithValue("$userId", document.UserId);
                command.Parameters.AddWithValue("$clientId", document.ClientId);
                command.Parameters.AddWithValue("$title", document.Title);
                command.Parameters.AddWithValue("$filePath", document.FilePath);
                command.Parameters.AddWithValue("$createdAt", document.CreatedAt.ToString("o"));
                command.Parameters.AddWithValue("$id", document.Id);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM documents WHERE id = $id;";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
