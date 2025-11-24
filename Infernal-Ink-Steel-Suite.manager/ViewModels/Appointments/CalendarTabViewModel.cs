using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using InfernalInkSteelSuite.Views.Appointments;
using InfernalInkSteelSuite.Views;

namespace InfernalInkSteelSuite.ViewModels.Appointments
{
    public class CalendarTabViewModel : BaseViewModel
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private DateTime _currentDate = DateTime.Today;
        private ObservableCollection<CalendarDay> _days = [];

        public CalendarTabViewModel(IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            PreviousMonthCommand = new RelayCommand(_ => PreviousMonth());
            NextMonthCommand = new RelayCommand(_ => NextMonth());
            TodayCommand = new RelayCommand(_ => GoToToday());
            DayClickedCommand = new RelayCommand(DayClicked);
            AddAppointmentCommand = new RelayCommand(AddAppointment);
            GenerateCalendar();
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentMonthYear));
                GenerateCalendar();
            }
        }

        public string CurrentMonthYear => CurrentDate.ToString("MMMM yyyy");

        public ObservableCollection<CalendarDay> Days
        {
            get => _days;
            set
            {
                _days = value;
                OnPropertyChanged();
            }
        }

        public ICommand PreviousMonthCommand { get; }
        public ICommand NextMonthCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand DayClickedCommand { get; }
        public ICommand AddAppointmentCommand { get; }

        private void PreviousMonth()
        {
            CurrentDate = CurrentDate.AddMonths(-1);
        }

        private void NextMonth()
        {
            CurrentDate = CurrentDate.AddMonths(1);
        }

        private void GoToToday()
        {
            CurrentDate = DateTime.Today;
        }

        private void AddAppointment(object? parameter)
        {
            if (parameter is DateTime date)
            {
                var newAppointment = new Appointment
                {
                    DateTime = date.Date.AddHours(12) // Default to noon
                };
                var dialog = new AppointmentDialog(_appointmentRepository, _clientRepository, newAppointment);

                if (dialog.ShowDialog() == true)
                {
                    GenerateCalendar();
                }
            }
        }

        private void DayClicked(object? parameter)
        {
            if (parameter is CalendarDay day && day.Appointments.Any())
            {
                var dialog = new AppointmentDetailsDialog
                {
                    DataContext = new AppointmentDetailsDialogViewModel(day.Date, day.Appointments)
                };
                dialog.ShowDialog();
            }
        }

        private void GenerateCalendar()
        {
            Days.Clear();
            var firstDayOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
            int startOffset = (int)firstDayOfMonth.DayOfWeek;

            var startDate = firstDayOfMonth.AddDays(-startOffset);

            var appointments = _appointmentRepository.GetAppointmentsByDateRange(startDate, startDate.AddDays(42));

            for (int i = 0; i < 42; i++)
            {
                var date = startDate.AddDays(i);
                var day = new CalendarDay
                {
                    Date = date,
                    IsInCurrentMonth = date.Month == CurrentDate.Month
                };
                var appointmentsForDay = appointments.Where(a => a.DateTime.Date == date.Date);
                foreach (var appointment in appointmentsForDay)
                {
                    day.Appointments.Add(appointment);
                }
                Days.Add(day);
            }
        }
    }
}
