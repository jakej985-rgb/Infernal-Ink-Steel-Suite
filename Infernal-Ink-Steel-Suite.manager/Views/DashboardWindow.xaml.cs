using System.Windows;

namespace InfernalInkSteelSuite.Views
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            MainContent.Content = new HomeView();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new HomeView();
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ClientsView();
        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AppointmentsView();
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
