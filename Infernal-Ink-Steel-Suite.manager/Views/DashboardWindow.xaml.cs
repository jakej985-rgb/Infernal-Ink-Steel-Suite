using InfernalInkSteelSuite.Repositories;
using System.Windows;

namespace InfernalInkSteelSuite.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly string _connectionString;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;

        public DashboardWindow(string connectionString)
        {
            _connectionString = connectionString;
            _appointmentRepository = new AppointmentRepository(_connectionString);
            _clientRepository = new ClientRepository(_connectionString);

            InitializeComponent();
            MainContent.Content = new HomeView();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new HomeView();
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ClientsView(_connectionString);
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AppointmentsView(_appointmentRepository, _clientRepository);
        }

        private void Quotes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new QuotesView();
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DocumentsView();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }
    }
}
