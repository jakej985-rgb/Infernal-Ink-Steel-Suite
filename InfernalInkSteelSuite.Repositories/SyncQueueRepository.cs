using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Domain;
using System.Threading.Tasks;

namespace InfernalInkSteelSuite.Repositories
{
    public class SyncQueueRepository(string connectionString) : ISyncQueueRepository
    {
        private readonly string _connectionString = connectionString;

        public void Enqueue(SyncQueueItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO sync_queue (entityType, entityId, action, payloadJson, createdAt, status)
                  VALUES ($entityType, $entityId, $action, $payloadJson, $createdAt, $status)";

            command.Parameters.AddWithValue("$entityType", item.EntityType);
            command.Parameters.AddWithValue("$entityId", item.EntityId);
            command.Parameters.AddWithValue("$action", item.Action);
            command.Parameters.AddWithValue("$payloadJson", item.PayloadJson != null ? item.PayloadJson : DBNull.Value);
            command.Parameters.AddWithValue("$createdAt", item.CreatedAt.ToString("o"));
            command.Parameters.AddWithValue("$status", "Pending");

            command.ExecuteNonQuery();
        }

        public List<SyncQueueItem> GetPendingItems()
        {
            var list = new List<SyncQueueItem>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT id, entityType, entityId, action, payloadJson, createdAt, status, retryCount FROM sync_queue WHERE status = 'Pending' ORDER BY createdAt ASC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SyncQueueItem
                {
                    Id = reader.GetInt32(0),
                    EntityType = reader.GetString(1),
                    EntityId = reader.GetInt32(2),
                    Action = reader.GetString(3),
                    PayloadJson = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = DateTime.Parse(reader.GetString(5)),
                    Status = reader.GetString(6),
                    RetryCount = reader.GetInt32(7)
                });
            }
            return list;
        }

        public void UpdateStatus(int id, string status, string? errorMessage = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE sync_queue SET status = $status, lastErrorMessage = $error WHERE id = $id";
            command.Parameters.AddWithValue("$status", status);
            command.Parameters.AddWithValue("$error", errorMessage != null ? errorMessage : DBNull.Value);
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        public void MarkAsSynced(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE sync_queue SET status = 'Synced', syncedAt = $syncedAt WHERE id = $id";
            command.Parameters.AddWithValue("$syncedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        public async Task EnqueueAsync(SyncQueueItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
               @"INSERT INTO sync_queue (entityType, entityId, action, payloadJson, createdAt, status)
                  VALUES ($entityType, $entityId, $action, $payloadJson, $createdAt, $status)";

            command.Parameters.AddWithValue("$entityType", item.EntityType);
            command.Parameters.AddWithValue("$entityId", item.EntityId);
            command.Parameters.AddWithValue("$action", item.Action);
            command.Parameters.AddWithValue("$payloadJson", item.PayloadJson != null ? item.PayloadJson : DBNull.Value);
            command.Parameters.AddWithValue("$createdAt", item.CreatedAt.ToString("o"));
            command.Parameters.AddWithValue("$status", "Pending");

            await command.ExecuteNonQueryAsync();
        }
    }
}
