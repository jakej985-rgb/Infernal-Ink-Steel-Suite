using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.UI.ViewModels;
using System;
using System.Windows;

namespace InfernalInkSteelSuite.UI.ViewModels.Settings
{
    public class ChangePasswordDialogViewModel : BaseViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly string _username;

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public Action? CloseAction { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ChangePasswordDialogViewModel(IUserRepository userRepository, string username)
        {
            _userRepository = userRepository;
            _username = username;
            _newPassword = string.Empty;
            _confirmPassword = string.Empty;
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(object? parameter)
        {
            if (NewPassword == ConfirmPassword)
            {
                _userRepository.UpdatePassword(_username, NewPassword);
                CloseAction?.Invoke();
            }
        }

        private void Cancel(object? parameter)
        {
            CloseAction?.Invoke();
        }
    }
}
