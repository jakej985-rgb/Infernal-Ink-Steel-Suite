using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.ObjectModel;

namespace InfernalInkSteelSuite.ViewModels.Appointments
{
    public class AppointmentDetailsDialogViewModel : BaseViewModel
    {
        private DateTime _date;
        private ObservableCollection<Appointment> _appointments;

        public AppointmentDetailsDialogViewModel(DateTime date, ObservableCollection<Appointment> appointments)
        {
            _date = date;
            _appointments = appointments;
        }

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
