using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IAppointmentRepository
    {
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void Delete(int id);
        Appointment? Get(int id);
        List<Appointment> GetAll();
        List<Appointment> GetAppointmentsByDate(DateTime date);
        List<Appointment> GetAppointmentsByUserId(int userId);
        List<Appointment> GetAppointmentsByClientId(int clientId);
        List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end);
        List<Appointment> GetAppointmentsByStatus(string status);
        List<Appointment> GetPaged(int page, int pageSize);
        Task<List<Appointment>> GetPagedAsync(int page, int pageSize);
    }
}
