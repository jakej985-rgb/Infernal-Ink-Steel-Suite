using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public class AppointmentRepository
    {
        private SqliteConnection GetConnection()
        {
            var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath
            }.ToString();
            return new SqliteConnection(connectionString);
        }

        public Appointment Get(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM appointments WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Appointment
                        {
                            Id = reader.GetInt32(0),
                            ClientId = reader.GetInt32(1),
                            UserId = reader.GetInt32(2),
                            DateTime = reader.GetDateTime(3),
                            DurationMinutes = reader.GetInt32(4),
                            ServiceType = reader.GetString(5),
                            ServiceCategory = reader.GetString(6),
                            PriceType = reader.GetString(7),
                            PriceCharged = reader.GetDecimal(8),
                            Notes = reader.GetString(9),
                            ClientName = reader.GetString(10),
                            Color = reader.GetString(11),
                            Status = reader.GetString(12)
                        };
                    }
                }
            }
            return null;
        }

        public List<Appointment> GetAll()
        {
            var appointments = new List<Appointment>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM appointments";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointments.Add(new Appointment
                        {
                            Id = reader.GetInt32(0),
                            ClientId = reader.GetInt32(1),
                            UserId = reader.GetInt32(2),
                            DateTime = reader.GetDateTime(3),
                            DurationMinutes = reader.GetInt32(4),
                            ServiceType = reader.GetString(5),
                            ServiceCategory = reader.GetString(6),
                            PriceType = reader.GetString(7),
                            PriceCharged = reader.GetDecimal(8),
                            Notes = reader.GetString(9),
                            ClientName = reader.GetString(10),
                            Color = reader.GetString(11),
                            Status = reader.GetString(12)
                        });
                    }
                }
            }
            return appointments;
        }

        public void Add(Appointment appointment)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO appointments (clientId, userId, dateTime, durationMinutes, serviceType, serviceCategory, priceType, priceCharged, notes, clientName, color, status) VALUES (@clientId, @userId, @dateTime, @durationMinutes, @serviceType, @serviceCategory, @priceType, @priceCharged, @notes, @clientName, @color, @status)";
                cmd.Parameters.AddWithValue("@clientId", appointment.ClientId);
                cmd.Parameters.AddWithValue("@userId", appointment.UserId);
                cmd.Parameters.AddWithValue("@dateTime", appointment.DateTime);
                cmd.Parameters.AddWithValue("@durationMinutes", appointment.DurationMinutes);
                cmd.Parameters.AddWithValue("@serviceType", appointment.ServiceType);
                cmd.Parameters.AddWithValue("@serviceCategory", appointment.ServiceCategory);
                cmd.Parameters.AddWithValue("@priceType", appointment.PriceType);
                cmd.Parameters.AddWithValue("@priceCharged", appointment.PriceCharged);
                cmd.Parameters.AddWithValue("@notes", appointment.Notes);
                cmd.Parameters.AddWithValue("@clientName", appointment.ClientName);
                cmd.Parameters.AddWithValue("@color", appointment.Color);
                cmd.Parameters.AddWithValue("@status", appointment.Status);
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Appointment appointment)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE appointments SET clientId = @clientId, userId = @userId, dateTime = @dateTime, durationMinutes = @durationMinutes, serviceType = @serviceType, serviceCategory = @serviceCategory, priceType = @priceType, priceCharged = @priceCharged, notes = @notes, clientName = @clientName, color = @color, status = @status WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", appointment.Id);
                cmd.Parameters.AddWithValue("@clientId", appointment.ClientId);
                cmd.Parameters.AddWithValue("@userId", appointment.UserId);
                cmd.Parameters.AddWithValue("@dateTime", appointment.DateTime);
                cmd.Parameters.AddWithValue("@durationMinutes", appointment.DurationMinutes);
                cmd.Parameters.AddWithValue("@serviceType", appointment.ServiceType);
                cmd.Parameters.AddWithValue("@serviceCategory", appointment.ServiceCategory);
                cmd.Parameters.AddWithValue("@priceType", appointment.PriceType);
                cmd.Parameters.AddWithValue("@priceCharged", appointment.PriceCharged);
                cmd.Parameters.AddWithValue("@notes", appointment.Notes);
                cmd.Parameters.AddWithValue("@clientName", appointment.ClientName);
                cmd.Parameters.AddWithValue("@color", appointment.Color);
                cmd.Parameters.AddWithValue("@status", appointment.Status);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM appointments WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
