using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace InfernalInkSteelSuite.UI.ViewModels
{
    public class AddEditClientViewModel : BaseViewModel
    {
        private readonly IClientRepository _clientRepository;
        private Client _client;

        public event EventHandler? RequestClose;

        public AddEditClientViewModel(IClientRepository clientRepository, Client client)
        {
            _clientRepository = clientRepository;
            _client = client;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public Client Client
        {
            get => _client;
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
                OnPropertyChanged(nameof(AvatarInitials));
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public string AvatarInitials
        {
            get
            {
                var initials = "";
                if (!string.IsNullOrWhiteSpace(Client.FirstName))
                    initials += Client.FirstName[0];
                if (!string.IsNullOrWhiteSpace(Client.LastName))
                    initials += Client.LastName[0];
                return string.IsNullOrWhiteSpace(initials) ? "?" : initials.ToUpper();
            }
        }


        private void Save()
        {
            ErrorMessage = string.Empty;

            if (Client.Id == 0)
            {
                // Check for duplicates
                var existingIdByName = _clientRepository.GetClientIdByName(Client.FullName);
                if (existingIdByName.HasValue)
                {
                    ErrorMessage = "A client with this name already exists.";
                    return;
                }

                if (!string.IsNullOrWhiteSpace(Client.Email))
                {
                    var existingIdByEmail = _clientRepository.GetClientIdByEmail(Client.Email);
                    if (existingIdByEmail.HasValue)
                    {
                        ErrorMessage = "A client with this email already exists.";
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(Client.Phone))
                {
                    var existingIdByPhone = _clientRepository.GetClientIdByPhone(Client.Phone);
                    if (existingIdByPhone.HasValue)
                    {
                        ErrorMessage = "A client with this phone number already exists.";
                        return;
                    }
                }

                _clientRepository.Insert(Client);
            }
            else
            {
                _clientRepository.Update(Client);
            }
            OnRequestClose();
        }

        private void Cancel()
        {
            OnRequestClose();
        }

        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
