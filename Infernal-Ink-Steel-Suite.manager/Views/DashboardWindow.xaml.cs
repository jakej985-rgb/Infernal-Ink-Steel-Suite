using Infernal_Ink_Steel_Suite.manager.ViewModels;
using Infernal_Ink_Steel_Suite.manager.Views;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.ViewModels;
using System.Windows;

namespace InfernalInkSteelSuite.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly string _connectionString;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDocumentRepository _documentRepository;

        public DashboardWindow(string connectionString)
        {
            _connectionString = connectionString;
            _appointmentRepository = new AppointmentRepository(_connectionString);
            _clientRepository = new ClientRepository(_connectionString);
            _documentRepository = new DocumentRepository(_connectionString);

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
            var documentsViewModel = new DocumentsViewModel(_documentRepository, _clientRepository);
            var documentsView = new DocumentsView();
            documentsView.DataContext = documentsViewModel;
            MainContent.Content = documentsView;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }
    }
}
