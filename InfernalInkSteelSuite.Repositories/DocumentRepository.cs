using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace InfernalInkSteelSuite.Repositories
{
    public class DocumentRepository
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbPath}");
        }

        public List<Document> GetAllDocuments()
        {
            var documents = new List<Document>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM documents";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        documents.Add(new Document
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ClientId = reader.GetInt32(2),
                            Title = reader.GetString(3),
                            FilePath = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }
            return documents;
        }

        public Document GetDocumentById(int id)
        {
            Document document = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM documents WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        document = new Document
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            ClientId = reader.GetInt32(2),
                            Title = reader.GetString(3),
                            FilePath = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5)
                        };
                    }
                }
            }
            return document;
        }

        public void AddDocument(Document document)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO documents (userId, clientId, title, filePath, createdAt)
                    VALUES (@userId, @clientId, @title, @filePath, @createdAt)
                ";
                command.Parameters.AddWithValue("@userId", document.UserId);
                command.Parameters.AddWithValue("@clientId", document.ClientId);
                command.Parameters.AddWithValue("@title", document.Title);
                command.Parameters.AddWithValue("@filePath", document.FilePath);
                command.Parameters.AddWithValue("@createdAt", document.CreatedAt);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateDocument(Document document)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE documents
                    SET userId = @userId, clientId = @clientId, title = @title, filePath = @filePath, createdAt = @createdAt
                    WHERE id = @id
                ";
                command.Parameters.AddWithValue("@id", document.Id);
                command.Parameters.AddWithValue("@userId", document.UserId);
                command.Parameters.AddWithValue("@clientId", document.ClientId);
                command.Parameters.AddWithValue("@title", document.Title);
                command.Parameters.AddWithValue("@filePath", document.FilePath);
                command.Parameters.AddWithValue("@createdAt", document.CreatedAt);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteDocument(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM documents WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
