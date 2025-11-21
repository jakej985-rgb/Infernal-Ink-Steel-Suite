using System.Windows;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Controls
{
    public partial class PasswordBoxWithToggle : UserControl
    {
        private bool _isUpdating;

        public PasswordBoxWithToggle()
        {
            InitializeComponent();
        }

        // The bound password (string)
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(PasswordBoxWithToggle),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBoxWithToggle control && !control._isUpdating)
            {
                control._isUpdating = true;
                var newPassword = e.NewValue as string ?? string.Empty;

                if (control.PwdBox.Password != newPassword)
                {
                    control.PwdBox.Password = newPassword;
                }

                if (control.TxtBox.Text != newPassword)
                {
                    control.TxtBox.Text = newPassword;
                }

                control._isUpdating = false;
            }
        }

        // Tracks whether the password is shown as plain text
        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register(
                nameof(IsPasswordVisible),
                typeof(bool),
                typeof(PasswordBoxWithToggle),
                new PropertyMetadata(false));

        public bool IsPasswordVisible
        {
            get => (bool)GetValue(IsPasswordVisibleProperty);
            set => SetValue(IsPasswordVisibleProperty, value);
        }

        private void ToggleBtn_OnClick(object sender, RoutedEventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;

            if (IsPasswordVisible)
            {
                // Show plain text
                TxtBox.Visibility = Visibility.Visible;
                PwdBox.Visibility = Visibility.Collapsed;
                TxtBox.Focus();
                TxtBox.SelectionStart = TxtBox.Text.Length;
            }
            else
            {
                // Show masked password
                TxtBox.Visibility = Visibility.Collapsed;
                PwdBox.Visibility = Visibility.Visible;
                PwdBox.Focus();
            }
        }

        private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isUpdating)
            {
                _isUpdating = true;
                Password = PwdBox.Password;
                TxtBox.Text = PwdBox.Password;
                _isUpdating = false;
            }
        }

        private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isUpdating)
            {
                _isUpdating = true;
                Password = TxtBox.Text;
                PwdBox.Password = TxtBox.Text;
                _isUpdating = false;
            }
        }
    }
}
