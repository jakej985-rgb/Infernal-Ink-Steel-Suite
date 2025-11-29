using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public class AppointmentRepository(string connectionString) : IAppointmentRepository
    {
        private readonly string _connectionString = connectionString;

        private SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            EnsureColumnExists(conn, "IsBlockOff", "INTEGER");
            return conn;
        }

        private static void EnsureColumnExists(SqliteConnection connection, string columnName, string columnType)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(appointments)";
            bool exists = false;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"]?.ToString()?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
            {
                var alter = connection.CreateCommand();
                alter.CommandText = $"ALTER TABLE appointments ADD COLUMN {columnName} {columnType} DEFAULT 0";
                alter.ExecuteNonQuery();
            }
        }

        private static Appointment MapReaderToAppointment(SqliteDataReader reader)
        {
            return new Appointment
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ClientId = reader.GetInt32(reader.GetOrdinal("clientId")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                ClientName = reader.GetString(reader.GetOrdinal("clientName")),
                DateTime = reader.GetDateTime(reader.GetOrdinal("dateTime")),
                DurationMinutes = reader.GetInt32(reader.GetOrdinal("durationMinutes")),
                ServiceType = reader.GetString(reader.GetOrdinal("serviceType")),
                ServiceCategory = reader.GetString(reader.GetOrdinal("serviceCategory")),
                PriceType = reader.GetString(reader.GetOrdinal("priceType")),
                PriceCharged = reader.GetDecimal(reader.GetOrdinal("priceCharged")),
                Notes = reader.GetString(reader.GetOrdinal("notes")),
                Color = reader.GetString(reader.GetOrdinal("color")),
                Status = reader.GetString(reader.GetOrdinal("status")),
                IsBlockOff = reader.GetInt32(reader.GetOrdinal("IsBlockOff")) == 1
            };
        }

        public Appointment? Get(int id)
        {
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToAppointment(reader);
            }
            return null;
        }

        public List<Appointment> GetAll()
        {
            var appointments = new List<Appointment>();
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments ORDER BY dateTime ASC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapReaderToAppointment(reader));
            }
            return appointments;
        }

        public void Add(Appointment appointment)
        {
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO appointments (clientId, userId, clientName, dateTime, durationMinutes, serviceType, serviceCategory, priceType, priceCharged, notes, color, status, IsBlockOff) VALUES (@clientId, @userId, @clientName, @dateTime, @durationMinutes, @serviceType, @serviceCategory, @priceType, @priceCharged, @notes, @color, @status, @IsBlockOff)";
            cmd.Parameters.AddWithValue("@clientId", appointment.ClientId);
            cmd.Parameters.AddWithValue("@userId", appointment.UserId);
            cmd.Parameters.AddWithValue("@clientName", appointment.ClientName);
            cmd.Parameters.AddWithValue("@dateTime", appointment.DateTime);
            cmd.Parameters.AddWithValue("@durationMinutes", appointment.DurationMinutes);
            cmd.Parameters.AddWithValue("@serviceType", appointment.ServiceType);
            cmd.Parameters.AddWithValue("@serviceCategory", appointment.ServiceCategory);
            cmd.Parameters.AddWithValue("@priceType", appointment.PriceType);
            cmd.Parameters.AddWithValue("@priceCharged", appointment.PriceCharged);
            cmd.Parameters.AddWithValue("@notes", appointment.Notes);
            cmd.Parameters.AddWithValue("@color", appointment.Color);
            cmd.Parameters.AddWithValue("@status", appointment.Status);
            cmd.Parameters.AddWithValue("@IsBlockOff", appointment.IsBlockOff ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void Update(Appointment appointment)
        {
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE appointments SET clientId = @clientId, userId = @userId, clientName = @clientName, dateTime = @dateTime, durationMinutes = @durationMinutes, serviceType = @serviceType, serviceCategory = @serviceCategory, priceType = @priceType, priceCharged = @priceCharged, notes = @notes, color = @color, status = @status, IsBlockOff = @IsBlockOff WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", appointment.Id);
            cmd.Parameters.AddWithValue("@clientId", appointment.ClientId);
            cmd.Parameters.AddWithValue("@userId", appointment.UserId);
            cmd.Parameters.AddWithValue("@clientName", appointment.ClientName);
            cmd.Parameters.AddWithValue("@dateTime", appointment.DateTime);
            cmd.Parameters.AddWithValue("@durationMinutes", appointment.DurationMinutes);
            cmd.Parameters.AddWithValue("@serviceType", appointment.ServiceType);
            cmd.Parameters.AddWithValue("@serviceCategory", appointment.ServiceCategory);
            cmd.Parameters.AddWithValue("@priceType", appointment.PriceType);
            cmd.Parameters.AddWithValue("@priceCharged", appointment.PriceCharged);
            cmd.Parameters.AddWithValue("@notes", appointment.Notes);
            cmd.Parameters.AddWithValue("@color", appointment.Color);
            cmd.Parameters.AddWithValue("@status", appointment.Status);
            cmd.Parameters.AddWithValue("@IsBlockOff", appointment.IsBlockOff ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM appointments WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            var appointments = new List<Appointment>();
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments WHERE DATE(dateTime) = @date ORDER BY dateTime ASC";
            cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapReaderToAppointment(reader));
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByUserId(int userId)
        {
            var appointments = new List<Appointment>();
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments WHERE userId = @userId ORDER BY dateTime ASC";
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapReaderToAppointment(reader));
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByClientId(int clientId)
        {
            var appointments = new List<Appointment>();
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments WHERE clientId = @clientId ORDER BY dateTime DESC";
            cmd.Parameters.AddWithValue("@clientId", clientId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapReaderToAppointment(reader));
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end)
        {
            var appointments = new List<Appointment>();
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments WHERE dateTime BETWEEN @start AND @end ORDER BY dateTime ASC";
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapReaderToAppointment(reader));
            }
            return appointments;
        }

        public List<Appointment> GetAppointmentsByStatus(string status)
        {
            var appointments = new List<Appointment>();
            using var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM appointments WHERE status = @status ORDER BY dateTime ASC";
            cmd.Parameters.AddWithValue("@status", status);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(MapReaderToAppointment(reader));
            }
            return appointments;
        }
    }
}
