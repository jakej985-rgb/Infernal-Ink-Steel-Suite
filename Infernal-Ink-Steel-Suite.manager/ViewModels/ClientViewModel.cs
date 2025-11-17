using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace InfernalInkSteelSuite.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
        private readonly IClientRepository _clientRepository;
        private string? _searchText;
        private List<Client> _allClients;
        private List<Client> _clients;

        public ClientViewModel(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            LoadClients();

            AddClientCommand = new RelayCommand(() => AddClient());
            EditClientCommand = new RelayCommand(p => EditClient(p));
        }

        public List<Client> Clients
        {
            get => _clients;
            private set
            {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        public string? SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                FilterClients();
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }

        private void LoadClients()
        {
            _allClients = _clientRepository.GetAll();
            Clients = _allClients;
        }

        private void FilterClients()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Clients = _allClients;
            }
            else
            {
                Clients = _allClients.Where(c => c.FullName.ToLower().Contains(SearchText.ToLower())).ToList();
            }
        }

        private void AddClient(object? parameter)
        private void AddClient()
        {
            var newClient = new Client();
            var viewModel = new AddEditClientViewModel(_clientRepository, newClient);
            var view = new AddEditClientView(viewModel);
            view.ShowDialog();
            LoadClients(); // Refresh the list
        }

        private void EditClient(object? parameter)
        {
            if (parameter is Client clientToEdit)
            {
                var viewModel = new AddEditClientViewModel(_clientRepository, clientToEdit);
                var view = new AddEditClientView(viewModel);
                view.ShowDialog();
                LoadClients(); // Refresh the list
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
