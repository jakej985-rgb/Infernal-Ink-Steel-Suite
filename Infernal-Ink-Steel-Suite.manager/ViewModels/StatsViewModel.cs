using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InfernalInkSteelSuite.Domain;
using System.Collections.ObjectModel;
using ScottPlot.WPF;
using ScottPlot;

namespace InfernalInkSteelSuite.ViewModels
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private IAppointmentRepository _appointmentRepository;
        private IShopSettingsRepository _shopSettingsRepository;

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

        public double[] IncomeData { get; set; }
        public double[] VisitsData { get; set; }
        public double[] HoursData { get; set; }

        public WpfPlot IncomeChart { get; set; }
        public WpfPlot VisitsChart { get; set; }
        public WpfPlot HoursChart { get; set; }

        public StatsViewModel(IAppointmentRepository appointmentRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _appointmentRepository = appointmentRepository;
            _shopSettingsRepository = shopSettingsRepository;

            IncomeChart = new WpfPlot();
            VisitsChart = new WpfPlot();
            HoursChart = new WpfPlot();

            Years = new ObservableCollection<int>();
            PopulateYearSelector();
            SelectedYear = Years.FirstOrDefault();
            LoadData(SelectedYear);
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
            VisitsData = visitsData.Select(v => (double)v).ToArray();
            HoursData = hoursData;

            TotalIncome = IncomeData.Sum();
            TotalVisits = visitsData.Sum();
            TotalHours = HoursData.Sum();

            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalVisits));
            OnPropertyChanged(nameof(TotalHours));

            UpdateCharts();
        }

        private void UpdateCharts()
        {
            IncomeChart.Plot.Clear();
            IncomeChart.Plot.Add.Bars(IncomeData);
            IncomeChart.Plot.XLabel("Month");
            IncomeChart.Plot.YLabel("Income");
            IncomeChart.Plot.Title($"Monthly Income - {SelectedYear}");
            IncomeChart.Refresh();

            VisitsChart.Plot.Clear();
            VisitsChart.Plot.Add.Bars(VisitsData);
            VisitsChart.Plot.XLabel("Month");
            VisitsChart.Plot.YLabel("Visits");
            VisitsChart.Plot.Title($"Monthly Visits - {SelectedYear}");
            VisitsChart.Refresh();

            HoursChart.Plot.Clear();
            HoursChart.Plot.Add.Bars(HoursData);
            HoursChart.Plot.XLabel("Month");
            HoursChart.Plot.YLabel("Hours");
            HoursChart.Plot.Title($"Monthly Tattoo Hours - {SelectedYear}");
            HoursChart.Refresh();
        }


        private bool ShouldCountAppointment(string status)
        {
            var normalized = status.Trim().ToLower();
            return !(normalized == "canceled" ||
                     normalized == "cancelled" ||
                     normalized == "no show" ||
                     normalized == "noshow");
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
