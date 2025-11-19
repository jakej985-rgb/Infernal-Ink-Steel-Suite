using InfernalInkSteelSuite.ViewModels;
using InfernalInkSteelSuite.Views;
using InfernalInkSteelSuite.Repositories;
using System.Windows;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Views.Dashboard;
using InfernalInkSteelSuite.ViewModels.Dashboard;

namespace InfernalInkSteelSuite.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly string _connectionString;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuoteRepository _quoteRepository;

        private readonly User _currentUser;

        public DashboardWindow(string connectionString, User currentUser)
        {
            _connectionString = connectionString;
            _currentUser = currentUser;
            _appointmentRepository = new AppointmentRepository(_connectionString);
            _clientRepository = new ClientRepository(_connectionString);
            _documentRepository = new DocumentRepository(_connectionString);
            _shopSettingsRepository = new ShopSettingsRepository(_connectionString);
            _userRepository = new UserRepository(_connectionString);
            _quoteRepository = new QuoteRepository(_connectionString);

            InitializeComponent();
            LoadShopSettings();
            var homeDashboardViewModel = new HomeDashboardViewModel(_currentUser, _shopSettingsRepository);
            var homeDashboardView = new HomeDashboardView();
            homeDashboardView.DataContext = homeDashboardViewModel;
            MainContent.Content = homeDashboardView;
            SettingsUpdateService.OnSettingsChanged += LoadShopSettings;
            Closed += (s, e) => SettingsUpdateService.OnSettingsChanged -= LoadShopSettings;
        }

        private void LoadShopSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            TattooRateLabel.Text = $"{settings.TattooPerHour:C}/hr";
            ShopMinimumRateLabel.Text = $"{settings.ShopMinimumRate:C}";
            PiercingRateLabel.Text = $"{settings.PiercingSingle:C}";
            if (!string.IsNullOrEmpty(settings.SidebarArtworkPath))
            {
                SidebarArtwork.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(settings.SidebarArtworkPath));
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var homeDashboardViewModel = new HomeDashboardViewModel(_currentUser, _shopSettingsRepository);
            var homeDashboardView = new HomeDashboardView();
            homeDashboardView.DataContext = homeDashboardViewModel;
            MainContent.Content = homeDashboardView;
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
            var quotesViewModel = new QuotesViewModel(_appointmentRepository);
            var quotesView = new QuotesView(_appointmentRepository);
            quotesView.DataContext = quotesViewModel;
            MainContent.Content = quotesView;
        }

        private void CreateQuote_Click(object sender, RoutedEventArgs e)
        {
            var pricingService = new TattooPricingService(_shopSettingsRepository, _userRepository);
            var quoteCreateViewModel = new QuoteCreateViewModel(pricingService, _quoteRepository);
            var quoteCreateView = new QuoteCreateView();
            quoteCreateView.DataContext = quoteCreateViewModel;
            MainContent.Content = quoteCreateView;
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
            var settingsViewModel = new SettingsViewModel(_connectionString, _currentUser);
            var settingsView = new SettingsView();
            settingsView.DataContext = settingsViewModel;
            MainContent.Content = settingsView;
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new StatsView();
        }
    }
}
