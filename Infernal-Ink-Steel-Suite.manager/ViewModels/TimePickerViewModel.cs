using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace InfernalInkSteelSuite.ViewModels
{
    public class TimePickerViewModel : BaseViewModel
    {
        public ObservableCollection<int> Hours { get; } = [];
        public ObservableCollection<string> Minutes { get; } = [];
        public ObservableCollection<string> AmPmOptions { get; } = ["AM", "PM"];

        private int _selectedHour = 12;
        public int SelectedHour
        {
            get => _selectedHour;
            set { _selectedHour = value; OnPropertyChanged(); }
        }

        private string _selectedMinute = "00";
        public string SelectedMinute
        {
            get => _selectedMinute;
            set { _selectedMinute = value; OnPropertyChanged(); }
        }

        private string _selectedAmPm = "PM";
        public string SelectedAmPm
        {
            get => _selectedAmPm;
            set { _selectedAmPm = value; OnPropertyChanged(); }
        }

        public TimePickerViewModel()
        {
            for (int i = 1; i <= 12; i++) Hours.Add(i);
            for (int i = 0; i < 60; i += 5) Minutes.Add(i.ToString("D2"));
        }

        public DateTime ApplyToDate(DateTime date)
        {
            int hour = SelectedHour;
            if (SelectedAmPm == "PM" && hour != 12) hour += 12;
            if (SelectedAmPm == "AM" && hour == 12) hour = 0;

            int minute = int.Parse(SelectedMinute);
            return date.Date + new TimeSpan(hour, minute, 0);
        }

        public void LoadFromDateTime(DateTime dt)
        {
            int hour = dt.Hour;
            SelectedAmPm = hour >= 12 ? "PM" : "AM";

            if (hour == 0) SelectedHour = 12;
            else if (hour > 12) SelectedHour = hour - 12;
            else SelectedHour = hour;

            string minStr = dt.Minute.ToString("D2");
            if (!Minutes.Contains(minStr))
            {
                Minutes.Add(minStr);
                var sorted = Minutes.OrderBy(x => x).ToList();
                Minutes.Clear();
                foreach (var m in sorted) Minutes.Add(m);
            }
            SelectedMinute = minStr;
        }
    }
}
