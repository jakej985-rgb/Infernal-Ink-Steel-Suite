using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class UpcomingAppointmentsTab : UserControl, IAppointmentTabView
    {
        public Appointment? SelectedAppointment => UpcomingAppointmentsGrid.SelectedItem as Appointment;

        private readonly IAppointmentRepository _appointmentRepository;

        public UpcomingAppointmentsTab(IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            Refresh();
        }

        public void Refresh()
        {
            UpcomingAppointmentsGrid.ItemsSource = _appointmentRepository.GetAppointmentsByStatus("Scheduled");
        }
    }
}
