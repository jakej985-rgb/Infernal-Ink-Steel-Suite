using NUnit.Framework;
using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;
using System;
using System.Linq;

namespace InfernalInkSteelSuite.Tests.ViewModels
{
    public class MockAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments;

        public MockAppointmentRepository(List<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end)
        {
            return _appointments.Where(a => a.DateTime >= start && a.DateTime <= end).ToList();
        }

        public List<Appointment> GetAll() => _appointments;
        public void Add(Appointment appointment) => throw new NotImplementedException();
        public void Update(Appointment appointment) => throw new NotImplementedException();
        public void Delete(int id) => throw new NotImplementedException();
        public Appointment? Get(int id) => throw new NotImplementedException();
        public List<Appointment> GetAppointmentsByDate(DateTime date) => throw new NotImplementedException();
        public List<Appointment> GetAppointmentsByUserId(int userId) => throw new NotImplementedException();
        public List<Appointment> GetAppointmentsByClientId(int clientId) => throw new NotImplementedException();
        public List<Appointment> GetAppointmentsByStatus(string status) => throw new NotImplementedException();
    }

    public class MockShopSettingsRepository : IShopSettingsRepository
    {
        private readonly ShopSettings _settings;

        public MockShopSettingsRepository(ShopSettings settings)
        {
            _settings = settings;
        }

        public ShopSettings LoadSettings() => _settings;
        public void SaveSettings(ShopSettings settings) => throw new NotImplementedException();
    }

    [TestFixture]
    public class StatsViewModelTests
    {
        [Test]
        public void LoadData_WhenCalled_CalculatesStatsCorrectly()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new() { Status = "Completed", PriceCharged = 100, DurationMinutes = 60, DateTime = new DateTime(2024, 1, 1) },
                new() { Status = "Cancelled", PriceCharged = 100, DurationMinutes = 60, DateTime = new DateTime(2024, 1, 1) },
                new() { Status = "No Show", PriceCharged = 100, DurationMinutes = 60, DateTime = new DateTime(2024, 1, 1) },
                new() { Status = "Completed", PriceCharged = 50, DurationMinutes = 30, DateTime = new DateTime(2024, 1, 1) },
                new() { Status = "scheduled", PriceCharged = 100, DurationMinutes = 60, DateTime = new DateTime(2024, 1, 1) },
            };
            var appointmentRepository = new MockAppointmentRepository(appointments);
            var shopSettingsRepository = new MockShopSettingsRepository(new ShopSettings { TattooPerHour = 100 });
            var viewModel = new StatsViewModel(appointmentRepository, shopSettingsRepository);

            // Act
            viewModel.LoadData(2024);

            // Assert
            Assert.AreEqual(150, viewModel.TotalIncome, "Total income should only include completed appointments.");
            Assert.AreEqual(2, viewModel.TotalVisits, "Total visits should only include completed appointments.");
            Assert.AreEqual(1.5, viewModel.TotalHours, "Total hours should only include completed appointments.");
        }
    }
}
