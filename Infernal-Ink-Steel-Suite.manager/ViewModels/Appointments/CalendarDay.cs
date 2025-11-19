using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.ObjectModel;

namespace InfernalInkSteelSuite.ViewModels.Appointments
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public int DayNumber => Date.Day;
        public bool IsInCurrentMonth { get; set; }
        public ObservableCollection<Appointment> Appointments { get; set; } = new();
    }
}
