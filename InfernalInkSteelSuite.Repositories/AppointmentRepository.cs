using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace InfernalInkSteelSuite.Repositories
{
    public class AppointmentRepository
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shop_manager.db");

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbPath}");
        }

        public List<Appointment> GetAllAppointments()
        {
            var appointments = new List<Appointment>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM appointments";
                using (var reader = command.ExecuteReader())
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
                            PriceCharged = reader.GetDouble(8),
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

        public Appointment GetAppointmentById(int id)
        {
            Appointment appointment = null;
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM appointments WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        appointment = new Appointment
                        {
                            Id = reader.GetInt32(0),
                            ClientId = reader.GetInt32(1),
                            UserId = reader.GetInt32(2),
                            DateTime = reader.GetDateTime(3),
                            DurationMinutes = reader.GetInt32(4),
                            ServiceType = reader.GetString(5),
                            ServiceCategory = reader.GetString(6),
                            PriceType = reader.GetString(7),
                            PriceCharged = reader.GetDouble(8),
                            Notes = reader.GetString(9),
                            ClientName = reader.GetString(10),
                            Color = reader.GetString(11),
                            Status = reader.GetString(12)
                        };
                    }
                }
            }
            return appointment;
        }

        public void AddAppointment(Appointment appointment)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO appointments (clientId, userId, dateTime, durationMinutes, serviceType, serviceCategory, priceType, priceCharged, notes, clientName, color, status)
                    VALUES (@clientId, @userId, @dateTime, @durationMinutes, @serviceType, @serviceCategory, @priceType, @priceCharged, @notes, @clientName, @color, @status)
                ";
                command.Parameters.AddWithValue("@clientId", appointment.ClientId);
                command.Parameters.AddWithValue("@userId", appointment.UserId);
                command.Parameters.AddWithValue("@dateTime", appointment.DateTime);
                command.Parameters.AddWithValue("@durationMinutes", appointment.DurationMinutes);
                command.Parameters.AddWithValue("@serviceType", appointment.ServiceType);
                command.Parameters.AddWithValue("@serviceCategory", appointment.ServiceCategory);
                command.Parameters.AddWithValue("@priceType", appointment.PriceType);
                command.Parameters.AddWithValue("@priceCharged", appointment.PriceCharged);
                command.Parameters.AddWithValue("@notes", appointment.Notes);
                command.Parameters.AddWithValue("@clientName", appointment.ClientName);
                command.Parameters.AddWithValue("@color", appointment.Color);
                command.Parameters.AddWithValue("@status", appointment.Status);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateAppointment(Appointment appointment)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE appointments
                    SET clientId = @clientId, userId = @userId, dateTime = @dateTime, durationMinutes = @durationMinutes, serviceType = @serviceType,
                        serviceCategory = @serviceCategory, priceType = @priceType, priceCharged = @priceCharged, notes = @notes,
                        clientName = @clientName, color = @color, status = @status
                    WHERE id = @id
                ";
                command.Parameters.AddWithValue("@id", appointment.Id);
                command.Parameters.AddWithValue("@clientId", appointment.ClientId);
                command.Parameters.AddWithValue("@userId", appointment.UserId);
                command.Parameters.AddWithValue("@dateTime", appointment.DateTime);
                command.Parameters.AddWithValue("@durationMinutes", appointment.DurationMinutes);
                command.Parameters.AddWithValue("@serviceType", appointment.ServiceType);
                command.Parameters.AddWithValue("@serviceCategory", appointment.ServiceCategory);
                command.Parameters.AddWithValue("@priceType", appointment.PriceType);
                command.Parameters.AddWithValue("@priceCharged", appointment.PriceCharged);
                command.Parameters.AddWithValue("@notes", appointment.Notes);
                command.Parameters.AddWithValue("@clientName", appointment.ClientName);
                command.Parameters.AddWithValue("@color", appointment.Color);
                command.Parameters.AddWithValue("@status", appointment.Status);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteAppointment(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM appointments WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
