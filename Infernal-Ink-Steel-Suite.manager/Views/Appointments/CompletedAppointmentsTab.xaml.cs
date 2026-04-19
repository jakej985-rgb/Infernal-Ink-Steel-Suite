using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows.Controls;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class CompletedAppointmentsTab : UserControl, IAppointmentTabView
    {
        public InfernalInkSteelSuite.Domain.Appointment? SelectedAppointment => CompletedAppointmentsGrid.SelectedItem as InfernalInkSteelSuite.Domain.Appointment;

        private readonly InfernalInkSteelSuite.Data.AppDbContext _db;
        private readonly InfernalInkSteelSuite.Repositories.IAppointmentRepository _appointmentRepository;
        private readonly InfernalInkSteelSuite.Repositories.IClientRepository _clientRepository;

        public CompletedAppointmentsTab(InfernalInkSteelSuite.Data.AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            _appointmentRepository = new InfernalInkSteelSuite.Repositories.AppointmentRepository(db);
            _clientRepository = new InfernalInkSteelSuite.Repositories.ClientRepository(db);
            Refresh();
        }

        public void Refresh()
        {
            CompletedAppointmentsGrid.ItemsSource = _appointmentRepository.GetAppointmentsByStatus("Completed");
        }

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CompletedAppointmentsGrid.SelectedItem is InfernalInkSteelSuite.Domain.Appointment appointment)
            {
                var dialog = new EditAppointmentDialog(appointment, _db);
                if (dialog.ShowDialog() == true)
                {
                    Refresh();
                }
            }
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CompletedAppointmentsGrid.SelectedItem is Appointment appointment)
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
            // Already completed, but kept for consistency or if we want to add "Mark as Pending" later
            if (CompletedAppointmentsGrid.SelectedItem is Appointment appointment)
            {
                appointment.Status = "Completed";
                _appointmentRepository.Update(appointment);
                Refresh();
            }
        }
    }
}
