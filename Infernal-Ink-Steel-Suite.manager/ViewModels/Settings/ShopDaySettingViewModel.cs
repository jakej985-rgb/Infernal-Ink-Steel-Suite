using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ShopDaySettingViewModel : BaseViewModel
    {
        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set { _isOpen = value; OnPropertyChanged(); }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(); }
        }

        public DayOfWeek Day { get; set; }

        public string DayName => Day.ToString();

        public ObservableCollection<string> TimeSlots { get; } = [];

        private string _selectedStartTime = "10:00 AM";
        public string SelectedStartTime
        {
            get => _selectedStartTime;
            set
            {
                _selectedStartTime = value;
                if (DateTime.TryParse(value, out var time)) StartTime = time;
                OnPropertyChanged();
            }
        }

        private string _selectedEndTime = "07:00 PM";
        public string SelectedEndTime
        {
            get => _selectedEndTime;
            set
            {
                _selectedEndTime = value;
                if (DateTime.TryParse(value, out var time)) EndTime = time;
                OnPropertyChanged();
            }
        }

        public ShopDaySettingViewModel()
        {
            // Generate time slots
            var start = DateTime.Today;
            for (int i = 0; i < 48; i++)
            {
                TimeSlots.Add(start.AddMinutes(i * 30).ToString("hh:mm tt"));
            }
        }
    }
}
