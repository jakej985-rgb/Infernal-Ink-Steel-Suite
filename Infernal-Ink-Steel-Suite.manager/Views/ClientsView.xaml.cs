using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly IClientRepository _clientRepo;
        public ObservableCollection<Client> Clients { get; } = [];

        public ClientsView(IClientRepository clientRepo)
        {
            InitializeComponent();
            _clientRepo = clientRepo;
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
            var newClient = new Client();
            var viewModel = new AddEditClientViewModel(_clientRepo, newClient);
            var dialog = new AddEditClientView(viewModel);

            viewModel.RequestClose += (s, args) =>
            {
                LoadClients(); // Refresh the grid
            };

            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void ClientsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenEditDialog();
        }

        private void ClientsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenEditDialog();
                e.Handled = true; // Prevent default behavior
            }
        }

        private void OpenEditDialog()
        {
            if (ClientsGrid.SelectedItem is Client selectedClient)
            {
                var viewModel = new AddEditClientViewModel(_clientRepo, selectedClient);
                var dialog = new AddEditClientView(viewModel);

                viewModel.RequestClose += (s, args) =>
                {
                    LoadClients(); // Refresh the grid
                };

                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
            }
        }
    }
}
