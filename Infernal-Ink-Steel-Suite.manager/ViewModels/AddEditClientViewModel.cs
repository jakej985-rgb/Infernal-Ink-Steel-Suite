using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows.Input;

namespace InfernalInkSteelSuite.ViewModels
{
    public class AddEditClientViewModel : BaseViewModel
    {
        private readonly IClientRepository _clientRepository;
        private Client _client;

        public event EventHandler? RequestClose;

        public AddEditClientViewModel(IClientRepository clientRepository, Client client)
        {
            _clientRepository = clientRepository;
            Client = client;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        public Client Client
        {
            get => _client;
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void Save(object? parameter)
        {
            if (Client.Id == 0)
            {
                _clientRepository.Insert(Client);
            }
            else
            {
                _clientRepository.Update(Client);
            }
            OnRequestClose();
        }

        private void Cancel(object? parameter)
        {
            OnRequestClose();
        }

        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
