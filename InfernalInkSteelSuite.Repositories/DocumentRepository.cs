using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public class DocumentRepository
    {
        public Document FromId(int docId)
        {
            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id, userId, clientId, title, filePath, createdAt FROM documents WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", docId);

                using (var reader = cmd.ExecuteReader())
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

        public bool Insert(Document document)
        {
            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    @"INSERT INTO documents (userId, clientId, title, filePath, createdAt)
                      VALUES ($userId, $clientId, $title, $filePath, $createdAt);";

                cmd.Parameters.AddWithValue("$userId", document.UserId);
                cmd.Parameters.AddWithValue("$clientId", document.ClientId);
                cmd.Parameters.AddWithValue("$title", document.Title);
                cmd.Parameters.AddWithValue("$filePath", document.FilePath);
                cmd.Parameters.AddWithValue("$createdAt", document.CreatedAt.ToString("o"));

                if (cmd.ExecuteNonQuery() == 1)
                {
                    // This is a bit of a hack to get the last inserted ID without another query.
                    // A more robust solution would be to use a separate query to get the last inserted ID.
                    cmd.CommandText = "select last_insert_rowid()";
                    document.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    return true;
                }
            }
            return false;
        }

        public bool Update(Document document)
        {
            if (document.Id == 0) return false;

            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    @"UPDATE documents SET
                          userId = $userId,
                          clientId = $clientId,
                          title = $title,
                          filePath = $filePath,
                          createdAt = $createdAt
                      WHERE id = $id;";

                cmd.Parameters.AddWithValue("$userId", document.UserId);
                cmd.Parameters.AddWithValue("$clientId", document.ClientId);
                cmd.Parameters.AddWithValue("$title", document.Title);
                cmd.Parameters.AddWithValue("$filePath", document.FilePath);
                cmd.Parameters.AddWithValue("$createdAt", document.CreatedAt.ToString("o"));
                cmd.Parameters.AddWithValue("$id", document.Id);

                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public bool Remove(Document document)
        {
            if (document.Id == 0) return false;

            using (var conn = Database.CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM documents WHERE id = $id;";
                cmd.Parameters.AddWithValue("$id", document.Id);

                return cmd.ExecuteNonQuery() == 1;
            }
        }
    }
}
