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
        public ObservableCollection<Appointment> Appointments { get; set; } = [];

        public bool HasAppointments => Appointments.Any();

        public List<CalendarAppointmentSummary> AppointmentSummaries
        {
            get
            {
                const int maxTimesToShow = 3;
                var sortedAppointments = Appointments.OrderBy(a => a.DateTime).ToList();
                var summaries = sortedAppointments.Take(maxTimesToShow).Select(a => new CalendarAppointmentSummary(a)).ToList();

                if (sortedAppointments.Count > maxTimesToShow)
                {
                    summaries.Add(new CalendarAppointmentSummary
                    {
                        DisplayText = $"+{sortedAppointments.Count - maxTimesToShow} more",
                        Color = "#808080", // Gray
                        TooltipText = "More appointments..."
                    });
                }
                return summaries;
            }
        }
    }

    public class CalendarAppointmentSummary
    {
        public string DisplayText { get; set; } = "";
        public string Color { get; set; } = "#000000";
        public string TooltipText { get; set; } = "";

        public CalendarAppointmentSummary() { }

        public CalendarAppointmentSummary(Appointment appointment)
        {
            DisplayText = appointment.DateTime.ToString("HH:mm");

            // Determine color based on status
            Color = GetColorForStatus(appointment.Status);

            // Build tooltip
            TooltipText = $"{appointment.DateTime:HH:mm} - {appointment.ClientName}\n{appointment.ServiceType}\nStatus: {appointment.Status}";
        }

        private static string GetColorForStatus(string status)
        {
            return status?.ToLower() switch
            {
                "scheduled" => "#4CAF50", // Green
                "confirmed" => "#4CAF50", // Green
                "pending" => "#FFC107",   // Amber
                "completed" => "#9E9E9E", // Gray
                "cancelled" => "#F44336", // Red
                "no show" => "#F44336",   // Red
                _ => "#2196F3"            // Blue (Default)
            };
        }
    }
}
