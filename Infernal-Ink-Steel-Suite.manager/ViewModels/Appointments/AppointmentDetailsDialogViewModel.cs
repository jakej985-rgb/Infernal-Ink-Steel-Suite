using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.ObjectModel;

namespace InfernalInkSteelSuite.ViewModels.Appointments
{
    public class AppointmentDetailsDialogViewModel(DateTime date, ObservableCollection<Appointment> appointments) : BaseViewModel
    {
        private DateTime _date = date;
        private ObservableCollection<Appointment> _appointments = appointments;

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set
            {
                _appointments = value;
                OnPropertyChanged();
            }
        }
    }
}
