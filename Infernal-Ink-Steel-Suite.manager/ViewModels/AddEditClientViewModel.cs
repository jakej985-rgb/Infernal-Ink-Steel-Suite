using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
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
            _client = client;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
            SelectPhotoCommand = new RelayCommand(_ => SelectPhoto());
            ClearPhotoCommand = new RelayCommand(_ => ClearPhoto());
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
        public ICommand SelectPhotoCommand { get; }
        public ICommand ClearPhotoCommand { get; }

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

        private void SelectPhoto()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Client Photo",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Create a directory for client photos if it doesn't exist
                    var photosDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientPhotos");
                    Directory.CreateDirectory(photosDir);

                    // Generate a unique filename based on client ID or timestamp
                    var extension = Path.GetExtension(dialog.FileName);
                    var fileName = $"client_{Client.Id}_{DateTime.Now.Ticks}{extension}";
                    var destPath = Path.Combine(photosDir, fileName);

                    // Copy the file to our photos directory
                    File.Copy(dialog.FileName, destPath, true);

                    // Update the client's photo path
                    Client.PhotoPath = destPath;
                    OnPropertyChanged(nameof(Client));
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to save photo: {ex.Message}";
                }
            }
        }

        private void ClearPhoto()
        {
            if (!string.IsNullOrWhiteSpace(Client.PhotoPath))
            {
                // Optionally delete the old file
                try
                {
                    if (File.Exists(Client.PhotoPath))
                    {
                        File.Delete(Client.PhotoPath);
                    }
                }
                catch
                {
                    // Ignore errors when deleting old photo
                }

                Client.PhotoPath = "";
                OnPropertyChanged(nameof(Client));
            }
        }


        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
