using System;

namespace InfernalInkSteelSuite.UI.ViewModels.Dashboard
{
    public class DaySummaryVm : BaseViewModel
    {
        public DateTime Date { get; set; }
        public string DayLabel { get; set; }
        public int DateNumber { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public DaySummaryVm()
        {
            DayLabel = string.Empty;
        }
    }
}
