using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.UI.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace InfernalInkSteelSuite.UI.ViewModels.Settings
{
    public class ChangeAvatarDialogViewModel : BaseViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly string _username;

        public ObservableCollection<string> DefaultAvatars { get; }

        private string _selectedAvatar;
        public string SelectedAvatar
        {
            get => _selectedAvatar;
            set
            {
                _selectedAvatar = value;
                OnPropertyChanged();
            }
        }

        public Action? CloseAction { get; set; }

        public RelayCommand BrowseCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ChangeAvatarDialogViewModel(IUserRepository userRepository, string username)
        {
            _userRepository = userRepository;
            _username = username;
            _selectedAvatar = string.Empty;

            DefaultAvatars = [];
            LoadDefaultAvatars();

            BrowseCommand = new RelayCommand(Browse);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadDefaultAvatars()
        {
            var avatarDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "Avatars");
            if (Directory.Exists(avatarDir))
            {
                foreach (var file in Directory.GetFiles(avatarDir, "*.png"))
                {
                    DefaultAvatars.Add(file);
                }
            }
        }

        private void Browse(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedAvatar = openFileDialog.FileName;
            }
        }

        private void Save(object? parameter)
        {
            _userRepository.UpdateAvatarPath(_username, SelectedAvatar);
            CloseAction?.Invoke();
        }

        private void Cancel(object? parameter)
        {
            CloseAction?.Invoke();
        }
    }
}
