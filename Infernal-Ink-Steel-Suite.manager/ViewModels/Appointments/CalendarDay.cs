using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InfernalInkSteelSuite.ViewModels.Appointments
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public int DayNumber => Date.Day;
        public bool IsInCurrentMonth { get; set; }
        public ObservableCollection<Appointment> Appointments { get; set; } = new();

        public bool HasAppointments => Appointments.Any();

        public List<string> AppointmentTimesSummary
        {
            get
            {
                const int maxTimesToShow = 3;
                var times = Appointments.OrderBy(a => a.DateTime)
                                        .Select(a => a.DateTime.ToString("HH:mm"))
                                        .ToList();

                if (times.Count <= maxTimesToShow)
                {
                    return times;
                }

                var summary = times.Take(maxTimesToShow).ToList();
                summary.Add($"+{times.Count - maxTimesToShow} more");
                return summary;
            }
        }
    }
}
