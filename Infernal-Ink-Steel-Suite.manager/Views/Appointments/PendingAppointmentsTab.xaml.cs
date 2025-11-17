using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class PendingAppointmentsTab : UserControl, IAppointmentTabView
    {
        public Appointment? SelectedAppointment => PendingAppointmentsGrid.SelectedItem as Appointment;

        private readonly IAppointmentRepository _appointmentRepository;

        public PendingAppointmentsTab(IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            Refresh();
        }

        public void Refresh()
        {
            PendingAppointmentsGrid.ItemsSource = _appointmentRepository.GetAppointmentsByStatus("Pending");
        }
    }
}
