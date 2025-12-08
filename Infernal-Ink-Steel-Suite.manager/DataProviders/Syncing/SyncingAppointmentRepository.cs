using System;
using System.Collections.Generic;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.DataProviders.Syncing
{
    public class SyncingAppointmentRepository : IAppointmentRepository
    {
        private readonly IAppointmentRepository _inner;
        private readonly ISyncQueueRepository _syncQueue;

        public SyncingAppointmentRepository(IAppointmentRepository inner, ISyncQueueRepository syncQueue)
        {
            _inner = inner;
            _syncQueue = syncQueue;
        }

        public void Add(Appointment appointment)
        {
            _inner.Add(appointment);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Appointment",
                EntityId = appointment.Id,
                Action = "Create",
                PayloadJson = JsonSerializer.Serialize(appointment)
            });
        }

        public void Update(Appointment appointment)
        {
            _inner.Update(appointment);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Appointment",
                EntityId = appointment.Id,
                Action = "Update",
                PayloadJson = JsonSerializer.Serialize(appointment)
            });
        }

        public void Delete(int id)
        {
            _inner.Delete(id);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Appointment",
                EntityId = id,
                Action = "Delete"
            });
        }

        public Appointment? Get(int id) => _inner.Get(id);
        public List<Appointment> GetAll() => _inner.GetAll();
        public List<Appointment> GetAppointmentsByDate(DateTime date) => _inner.GetAppointmentsByDate(date);
        public List<Appointment> GetAppointmentsByUserId(int userId) => _inner.GetAppointmentsByUserId(userId);
        public List<Appointment> GetAppointmentsByClientId(int clientId) => _inner.GetAppointmentsByClientId(clientId);
        public List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end) => _inner.GetAppointmentsByDateRange(start, end);
        public List<Appointment> GetAppointmentsByStatus(string status) => _inner.GetAppointmentsByStatus(status);
    }
}
