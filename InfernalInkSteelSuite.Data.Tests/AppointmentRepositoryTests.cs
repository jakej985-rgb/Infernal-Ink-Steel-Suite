using Xunit;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;

namespace InfernalInkSteelSuite.Data.Tests
{
    [Collection("Database collection")]
    public class AppointmentRepositoryTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly AppointmentRepository _repository;

        public AppointmentRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new AppointmentRepository(_fixture.Context);
        }

        [Fact]
        public void Add_AppointmentOnSameDayButPastTime_ShouldNotThrowException()
        {
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

            var exception = Record.Exception(() => _repository.Add(appointment));

            Assert.Null(exception);
        }

        [Fact]
        public void Update_AppointmentOnSameDayButPastTime_ShouldNotThrowException()
        {
            var appointment = new Appointment
            {
                ClientId = 1,
                UserId = 1,
                ClientName = "Test Client",
                DateTime = DateTime.Now.AddDays(1),
                DurationMinutes = 60,
                ServiceType = "Test Service",
                ServiceCategory = "Test Category",
                PriceType = "Fixed",
                PriceCharged = 100,
                Notes = "",
                Color = "Blue",
                Status = "Scheduled"
            };

            _repository.Add(appointment);

            var savedAppointment = _repository.GetAppointmentsByClientId(1)[0];

            savedAppointment.DateTime = DateTime.Now.AddHours(-1);

            var exception = Record.Exception(() => _repository.Update(savedAppointment));

            Assert.Null(exception);
        }
    }
}
