using InfernalInkSteelSuite.Repositories;
using System.Windows;
using System.Windows.Controls;

namespace Infernal_Ink_Steel_Suite.manager
{
    public partial class AppointmentsPage : Page
    {
        private readonly AppointmentRepository _appointmentRepository;

        public AppointmentsPage()
        {
            InitializeComponent();
            _appointmentRepository = new AppointmentRepository();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            UpcomingAppointmentsGrid.ItemsSource = _appointmentRepository.GetAllAppointments();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Add appointment logic here
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Edit appointment logic here
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Delete appointment logic here
        }
    }
}
