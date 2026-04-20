using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InfernalInkSteelSuite.Domain;
using System.Collections.ObjectModel;

namespace InfernalInkSteelSuite.ViewModels
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;

        public ObservableCollection<int> Years { get; set; }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                    LoadData(_selectedYear);
                }
            }
        }

        public double TotalIncome { get; set; }
        public int TotalVisits { get; set; }
        public double TotalHours { get; set; }

        private double[] _incomeData = [];
        public double[] IncomeData
        {
            get => _incomeData;
            set { _incomeData = value; OnPropertyChanged(nameof(IncomeData)); }
        }

        private double[] _visitsData = [];
        public double[] VisitsData
        {
            get => _visitsData;
            set { _visitsData = value; OnPropertyChanged(nameof(VisitsData)); }
        }

        private double[] _hoursData = [];
        public double[] HoursData
        {
            get => _hoursData;
            set { _hoursData = value; OnPropertyChanged(nameof(HoursData)); }
        }

        public StatsViewModel(IAppointmentRepository appointmentRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _appointmentRepository = appointmentRepository;
            _shopSettingsRepository = shopSettingsRepository;

            Years = [];
            PopulateYearSelector();
            _selectedYear = Years.FirstOrDefault();
            LoadData(_selectedYear);
        }

        public void PopulateYearSelector()
        {
            var years = _appointmentRepository.GetAll()
                .Select(a => a.DateTime.Year)
                .Distinct()
                .OrderByDescending(y => y);

            foreach (var year in years)
            {
                Years.Add(year);
            }

            if (!Years.Any())
            {
                Years.Add(DateTime.Now.Year);
            }
        }

        public void LoadData(int year)
        {
            var appointments = _appointmentRepository.GetAppointmentsByDateRange(new DateTime(year, 1, 1), new DateTime(year, 12, 31));
            var settings = _shopSettingsRepository.LoadSettings();
            var hourlyRate = settings.TattooPerHour;

            var incomeData = new double[12];
            var visitsData = new int[12];
            var hoursData = new double[12];

            foreach (var appt in appointments)
            {
                if (!ShouldCountAppointment(appt.Status))
                {
                    continue;
                }

                var monthIndex = appt.DateTime.Month - 1;

                visitsData[monthIndex]++;
                hoursData[monthIndex] += appt.DurationMinutes / 60.0;

                double income = (double)appt.PriceCharged;
                if (income == 0 && hourlyRate > 0)
                {
                    income = (appt.DurationMinutes / 60.0) * hourlyRate;
                }
                incomeData[monthIndex] += income;
            }

            IncomeData = incomeData;
            VisitsData = [.. visitsData.Select(v => (double)v)];
            HoursData = hoursData;

            TotalIncome = IncomeData.Sum();
            TotalVisits = visitsData.Sum();
            TotalHours = HoursData.Sum();

            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalVisits));
            OnPropertyChanged(nameof(TotalHours));
        }


        private static bool ShouldCountAppointment(string status)
        {
            var trimmedStatus = status.Trim();
            return trimmedStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
                   trimmedStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
