using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class CalendarTab : UserControl, IAppointmentTabView
    {
        public Appointment? SelectedAppointment => AppointmentsGrid.SelectedItem as Appointment;

        private readonly IAppointmentRepository _appointmentRepository;

        public CalendarTab(IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            AppointmentCalendar.SelectedDate = DateTime.Today;
            Refresh();
        }

        public void Refresh()
        {
            if (AppointmentCalendar.SelectedDate.HasValue)
            {
                AppointmentsGrid.ItemsSource = _appointmentRepository.GetAppointmentsByDate(AppointmentCalendar.SelectedDate.Value);
            }
        }

        private void AppointmentCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
    }
}
