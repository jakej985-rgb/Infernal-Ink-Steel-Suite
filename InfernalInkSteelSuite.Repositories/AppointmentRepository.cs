using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfernalInkSteelSuite.Repositories
{
    public class AppointmentRepository(AppDbContext dbContext) : IAppointmentRepository
    {
        private readonly AppDbContext _db = dbContext;

        public void Add(Appointment appointment)
        {
            _db.Appointments.Add(appointment);
            _db.SaveChanges();
        }

        public void Update(Appointment appointment)
        {
            _db.Appointments.Update(appointment);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var appt = _db.Appointments.Find(id);
            if (appt != null)
            {
                _db.Appointments.Remove(appt);
                _db.SaveChanges();
            }
        }

        public Appointment? Get(int id)
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .FirstOrDefault(a => a.Id == id);
        }

        public List<Appointment> GetAll()
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .OrderBy(a => a.DateTime)
                .ToList();
        }

        public List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            var d = date.Date;
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .Where(a => a.DateTime.Date == d)
                .OrderBy(a => a.DateTime)
                .ToList();
        }

        public List<Appointment> GetAppointmentsByUserId(int userId)
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.DateTime)
                .ToList();
        }

        public List<Appointment> GetAppointmentsByClientId(int clientId)
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.DateTime)
                .ToList();
        }

        public List<Appointment> GetAppointmentsByDateRange(DateTime start, DateTime end)
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .Where(a => a.DateTime >= start && a.DateTime < end)
                .OrderBy(a => a.DateTime)
                .ToList();
        }

        public List<Appointment> GetAppointmentsByStatus(string status)
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .Where(a => a.Status == status)
                .OrderBy(a => a.DateTime)
                .ToList();
        }

        public List<Appointment> GetPaged(int page, int pageSize)
        {
            return _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .OrderByDescending(a => a.DateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<List<Appointment>> GetPagedAsync(int page, int pageSize)
        {
            return await _db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .OrderByDescending(a => a.DateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public int CountByDate(DateTime date)
        {
            var d = date.Date;
            return _db.Appointments.Count(a => a.DateTime.Date == d);
        }

        public int CountUpcoming()
        {
            var now = DateTime.UtcNow;
            return _db.Appointments.Count(a => a.DateTime > now);
        }

        public Dictionary<DateTime, int> GetHeatmapData(DateTime start, DateTime end)
        {
            return _db.Appointments
                .Where(a => a.DateTime >= start && a.DateTime < end)
                .GroupBy(a => a.DateTime.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Date, x => x.Count);
        }
    }
}
