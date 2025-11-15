using InfernalInkSteelSuite.Repositories;
using System.Windows;
using System.Windows.Controls;

namespace Infernal_Ink_Steel_Suite.manager
{
    public partial class ClientPage : Page
    {
        private readonly ClientRepository _clientRepository;

        public ClientPage()
        {
            InitializeComponent();
            _clientRepository = new ClientRepository();
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsGrid.ItemsSource = _clientRepository.GetAllClients();
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Add client logic here
        }
    }
}
