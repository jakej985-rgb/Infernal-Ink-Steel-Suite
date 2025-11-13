using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly ClientRepository _clientRepo = new ClientRepository();
        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();

        public ClientsView()
        {
            InitializeComponent();
            LoadClients();
            ClientsGrid.ItemsSource = Clients;
        }

        private void LoadClients()
        {
            Clients.Clear();
            var list = _clientRepo.GetAll();
            foreach (var c in list)
                Clients.Add(c);
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add client dialog / page
            MessageBox.Show("Add client not implemented yet.");
        }
    }
}
