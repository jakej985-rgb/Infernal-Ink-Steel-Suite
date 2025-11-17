using Xunit;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;
using System;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class AppointmentRepositoryTests
    {
        private const string ConnectionString = "DataSource=:memory:";

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var databaseManager = new DatabaseManager(ConnectionString);
                databaseManager.InitializeDatabase();
            }
        }

        [Fact]
        public void Add_AppointmentOnSameDayButPastTime_ShouldNotThrowException()
        {
            CreateTable();
            var repository = new AppointmentRepository(ConnectionString);
            var appointment = new Appointment
            {
                ClientId = 1,
                UserId = 1,
                ClientName = "Test Client",
                DateTime = DateTime.Now.AddHours(-1),
                DurationMinutes = 60,
                ServiceType = "Test Service",
                ServiceCategory = "Test Category",
                PriceType = "Fixed",
                PriceCharged = 100,
                Notes = "",
                Color = "Blue",
                Status = "Scheduled"
            };

            var exception = Record.Exception(() => repository.Add(appointment));

            Assert.Null(exception);
        }
    }
}
