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
        private readonly IClientRepository _clientRepository;

        public UpcomingAppointmentsTab(IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            Refresh();
        }

        public void Refresh()
        {
            UpcomingAppointmentsGrid.ItemsSource = _appointmentRepository.GetAppointmentsByStatus("Scheduled");
        }

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (UpcomingAppointmentsGrid.SelectedItem is Appointment appointment)
            {
                var dialog = new EditAppointmentDialog(appointment, _appointmentRepository, _clientRepository);
                if (dialog.ShowDialog() == true)
                {
                    Refresh();
                }
            }
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (UpcomingAppointmentsGrid.SelectedItem is Appointment appointment)
            {
                if (System.Windows.MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Delete", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                {
                    _appointmentRepository.Delete(appointment.Id);
                    Refresh();
                }
            }
        }

        private void MarkCompleted_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (UpcomingAppointmentsGrid.SelectedItem is Appointment appointment)
            {
                appointment.Status = "Completed";
                _appointmentRepository.Update(appointment);
                Refresh();
            }
        }
    }
}
