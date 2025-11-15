using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class CompletedAppointmentsTab : UserControl, IAppointmentTabView
    {
        public Appointment SelectedAppointment => CompletedAppointmentsGrid.SelectedItem as Appointment;

        private readonly IAppointmentRepository _appointmentRepository;

        public CompletedAppointmentsTab(IAppointmentRepository appointmentRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            Refresh();
        }

        public void Refresh()
        {
            CompletedAppointmentsGrid.ItemsSource = _appointmentRepository.GetAppointmentsByStatus("Completed");
        }
    }
}
