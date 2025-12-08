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
        private readonly IDataProvider _dataProvider;
        private readonly User _currentUser;
        private readonly ImageComplexityService _imageComplexityService;

        public DashboardWindow(IDataProvider dataProvider, User currentUser)
        {
            _dataProvider = dataProvider;
            _currentUser = currentUser;

            InitializeComponent();

            // Set DataContext for Window (StatusBar etc)
            this.DataContext = new DashboardViewModel(new ConnectionSettingsService().Load());

            _imageComplexityService = new ImageComplexityService();
            LoadShopSettings();
            var homeDashboardViewModel = new HomeDashboardViewModel(_currentUser, _dataProvider.ShopSettings, _dataProvider.Appointments, _dataProvider.Clients);
            var homeDashboardView = new HomeDashboardView
            {
                DataContext = homeDashboardViewModel
            };
            MainContent.Content = homeDashboardView;
            SettingsUpdateService.OnSettingsChanged += LoadShopSettings;
            Closed += (s, e) => SettingsUpdateService.OnSettingsChanged -= LoadShopSettings;
        }

        private void LoadShopSettings()
        {
            var settings = _dataProvider.ShopSettings.LoadSettings();
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
            var login = new Login(_dataProvider);
            login.Show();
            this.Close();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var homeDashboardViewModel = new HomeDashboardViewModel(_currentUser, _dataProvider.ShopSettings, _dataProvider.Appointments, _dataProvider.Clients);
            var homeDashboardView = new HomeDashboardView
            {
                DataContext = homeDashboardViewModel
            };
            MainContent.Content = homeDashboardView;
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ClientsView(_dataProvider.Clients);
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AppointmentsView(_dataProvider.Appointments, _dataProvider.Clients, _dataProvider.ShopSettings);
        }

        private void Quotes_Click(object sender, RoutedEventArgs e)
        {
            var quotesViewModel = new QuotesViewModel(_dataProvider.Appointments);
            var quotesView = new QuotesView(_dataProvider.Appointments)
            {
                DataContext = quotesViewModel
            };
            MainContent.Content = quotesView;
        }

        private void CreateQuote_Click(object sender, RoutedEventArgs e)
        {
            var pricingService = new TattooPricingService(_dataProvider.ShopSettings, _dataProvider.Users);
            var quoteCreateViewModel = new QuoteCreateViewModel(pricingService, _dataProvider.Quotes, _dataProvider.Clients, _dataProvider.Users, _dataProvider.Appointments, _imageComplexityService, _dataProvider.ShopSettings);
            var quoteCreateView = new QuoteCreateView
            {
                DataContext = quoteCreateViewModel
            };
            MainContent.Content = quoteCreateView;
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            var documentsViewModel = new DocumentsViewModel(_dataProvider.Documents, _dataProvider.Clients);
            var documentsView = new DocumentsView
            {
                DataContext = documentsViewModel
            };
            MainContent.Content = documentsView;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsViewModel = new SettingsViewModel(_dataProvider, _currentUser);
            var settingsView = new SettingsView
            {
                DataContext = settingsViewModel
            };
            MainContent.Content = settingsView;
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new StatsView(_dataProvider);
        }
    }
}
