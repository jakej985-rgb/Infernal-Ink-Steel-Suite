using InfernalInkSteelSuite.ViewModels; // refresh
using InfernalInkSteelSuite.Views;
using InfernalInkSteelSuite.Repositories;
using System.Windows;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Views.Dashboard;
using InfernalInkSteelSuite.ViewModels.Dashboard;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly AppDbContext _db;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ClientRepository _clientRepository;
        private readonly DocumentRepository _documentRepository;
        private readonly ShopSettingsRepository _shopSettingsRepository;
        private readonly UserRepository _userRepository;
        private readonly QuoteRepository _quoteRepository;
        private readonly ImageComplexityService _imageComplexityService;

        private readonly User _currentUser;

        public DashboardWindow(AppDbContext db, User currentUser)
        {
            _db = db;
            _currentUser = currentUser;
            _appointmentRepository = new AppointmentRepository(_db);
            _clientRepository = new ClientRepository(_db);
            _documentRepository = new DocumentRepository(_db);
            _shopSettingsRepository = new ShopSettingsRepository(_db);
            var hasher = new InfernalInkSteelSuite.Repositories.Services.PasswordHasher();
            _userRepository = new UserRepository(_db, hasher);
            _quoteRepository = new QuoteRepository(_db);
            _imageComplexityService = new ImageComplexityService();

            InitializeComponent();
            LoadShopSettings();
            var homeDashboardViewModel = new HomeDashboardViewModel(_currentUser, _db);
            var homeDashboardView = new HomeDashboardView
            {
                DataContext = homeDashboardViewModel
            };
            MainContent.Content = homeDashboardView;
            SettingsUpdateService.OnSettingsChanged += LoadShopSettings;

            if (App.SyncService != null)
            {
                App.SyncService.OnSyncStatusChanged += UpdateSyncStatus;
                SyncStatusLabel.Text = "Ready";
            }

            Closed += (s, e) => 
            {
                SettingsUpdateService.OnSettingsChanged -= LoadShopSettings;
                if (App.SyncService != null)
                {
                    App.SyncService.OnSyncStatusChanged -= UpdateSyncStatus;
                }
            };
        }

        private void UpdateSyncStatus(string status)
        {
            Dispatcher.Invoke(() => SyncStatusLabel.Text = status);
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

        internal void Home_Click(object? sender, RoutedEventArgs? e)
        {
            var homeDashboardViewModel = new HomeDashboardViewModel(_currentUser, _db);
            var homeDashboardView = new HomeDashboardView
            {
                DataContext = homeDashboardViewModel
            };
            MainContent.Content = homeDashboardView;
        }

        internal void Clients_Click(object? sender, RoutedEventArgs? e)
        {
            MainContent.Content = new ClientsView(_db);
        }

        internal void Appointments_Click(object? sender, RoutedEventArgs? e)
        {
            MainContent.Content = new AppointmentsView(_db);
        }

        internal void Quotes_Click(object? sender, RoutedEventArgs? e)
        {
            var quotesViewModel = new QuotesViewModel(_appointmentRepository);
            var quotesView = new QuotesView(_appointmentRepository)
            {
                DataContext = quotesViewModel
            };
            MainContent.Content = quotesView;
        }

        private void CreateQuote_Click(object sender, RoutedEventArgs e)
        {
            var pricingService = new TattooPricingService(_shopSettingsRepository, _userRepository);
            var quoteCreateViewModel = new QuoteCreateViewModel(_db, pricingService, _quoteRepository, _clientRepository, _userRepository, _appointmentRepository, _imageComplexityService);
            var quoteCreateView = new QuoteCreateView
            {
                DataContext = quoteCreateViewModel
            };
            MainContent.Content = quoteCreateView;
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            var documentsViewModel = new DocumentsViewModel(_documentRepository, _clientRepository);
            var documentsView = new DocumentsView
            {
                DataContext = documentsViewModel
            };
            MainContent.Content = documentsView;
        }

        internal void Settings_Click(object? sender, RoutedEventArgs? e)
        {
            var settingsViewModel = new SettingsViewModel(_db, _currentUser);
            var settingsView = new SettingsView
            {
                DataContext = settingsViewModel
            };
            MainContent.Content = settingsView;
        }

        internal void Statistics_Click(object? sender, RoutedEventArgs? e)
        {
            MainContent.Content = new StatsView(_db);
        }
    }
}
