using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.ViewModels;
using System.Windows;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly string _connectionString;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;

        private readonly User _currentUser;

        public DashboardWindow(string connectionString, User currentUser)
        {
            _connectionString = connectionString;
            _currentUser = currentUser;
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
            var clientViewModel = new ClientViewModel(_clientRepository);
            var clientView = new ClientView();
            clientView.DataContext = clientViewModel;
            MainContent.Content = clientView;
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
            var settingsViewModel = new SettingsViewModel(_connectionString, _currentUser);
            var settingsView = new SettingsView();
            settingsView.DataContext = settingsViewModel;
            MainContent.Content = settingsView;
        }
    }
}
