using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InfernalInkSteelSuite.ViewModels
{
    public class ClientSearchViewModel : BaseViewModel
    {
        private readonly IClientRepository _clientRepository;
        private readonly List<Client> _allClients;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterClients();
            }
        }

        private ObservableCollection<Client> _filteredClients = [];
        public ObservableCollection<Client> FilteredClients
        {
            get => _filteredClients;
            set
            {
                _filteredClients = value;
                OnPropertyChanged();
            }
        }

        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
                if (value != null && string.IsNullOrEmpty(SearchText))
                {
                    // Optionally update search text to match selected client name if appropriate
                }
            }
        }

        public ClientSearchViewModel(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _allClients = [.. _clientRepository.GetAll()];
            FilterClients();
        }

        public void FilterClients()
        {
            FilteredClients.Clear();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var client in _allClients) FilteredClients.Add(client);
            }
            else
            {
                var search = SearchText.ToLower();
                var matches = _allClients.Where(c =>
                    c.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));

                foreach (var client in matches) FilteredClients.Add(client);
            }
        }

        public void SelectClient(int clientId)
        {
            SelectedClient = _allClients.FirstOrDefault(c => c.Id == clientId);
        }
    }
}
