using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Controls;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;
using InfernalInkSteelSuite.Views;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite
{
    public partial class Login : Window
    {
        private readonly UserRepository _userRepository;
        private readonly ShopSettingsRepository _settingsRepository;
        private User _currentUser;
        private readonly string _connectionString;

        public Login(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
            _userRepository = new UserRepository(connectionString);
            _settingsRepository = new ShopSettingsRepository(connectionString);
            BuildUserGrid();
            ApplyBranding();
        }

        private void BuildUserGrid()
        {
            UserGrid.Children.Clear();
            var users = _userRepository.GetAllUsers();
            foreach (var user in users)
            {
                var userWidget = new StackPanel { Margin = new Thickness(10) };
                var avatar = new GlowAvatar
                {
                    Size = 100,
                    Initials = user.Username.Substring(0, 1).ToUpper()
                };
                var nameLabel = new TextBlock
                {
                    Text = user.Username,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 6, 0, 0)
                };
                userWidget.Children.Add(avatar);
                userWidget.Children.Add(nameLabel);
                userWidget.MouseDown += (sender, e) => ShowUserSelected(user);
                UserGrid.Children.Add(userWidget);
            }
        }

        private void ApplyBranding()
        {
            ApplyBranding(BackButton);
        }

        private void ApplyBranding(Button backButton)
        {
            var settings = _settingsRepository.LoadSettings();

            Title = string.IsNullOrWhiteSpace(settings.ShopName) ? "Login" : $"{settings.ShopName} Login";
            HeadlineLabel.Text = string.IsNullOrWhiteSpace(settings.LoginHeadline) ? "Welcome Back" : settings.LoginHeadline;
            TaglineLabel.Text = string.IsNullOrWhiteSpace(settings.LoginTagline) ? "Sign in to manage your day." : settings.LoginTagline;

            if (!string.IsNullOrWhiteSpace(settings.LoginBackgroundPath) && File.Exists(settings.LoginBackgroundPath))
            {
                BackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(settings.LoginBackgroundPath)));
            }

            if (!string.IsNullOrWhiteSpace(settings.AccentColor))
            {
                var accentColor = (Color)ColorConverter.ConvertFromString(settings.AccentColor);
                SignInButton.Background = new SolidColorBrush(accentColor);
                backButton.Background = new SolidColorBrush(accentColor);

                var lighterAccent = GetLighterColor(accentColor, 1.3f);

                var gradient = new LinearGradientBrush();
                gradient.StartPoint = new Point(0, 0);
                gradient.EndPoint = new Point(0, 1);
                gradient.GradientStops.Add(new GradientStop(lighterAccent, 0.0));
                gradient.GradientStops.Add(new GradientStop(accentColor, 1.0));

                SignInButton.Background = gradient;
                BackButton.Background = gradient;
                PasswordEdit.BorderBrush = new SolidColorBrush(accentColor);
                PasswordTextBox.BorderBrush = new SolidColorBrush(accentColor);
            }
        }

        private void ShowUserSelected(User user)
        {
            _currentUser = user;
            AvatarInitials.Text = user.Username.Substring(0, 1).ToUpper();
            SelectedUserLabel.Text = user.Username;
            PasswordEdit.Clear();
            PasswordTextBox.Clear();
            UserScrollArea.Visibility = Visibility.Collapsed;
            UserLoginView.Visibility = Visibility.Visible;
            PasswordEdit.Focus();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var password = ShowPasswordCheck.IsChecked == true ? PasswordTextBox.Text : PasswordEdit.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_userRepository.CheckPassword(_currentUser.Username, password))
            {
                MessageBox.Show("Incorrect password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dashboard = new DashboardWindow(_connectionString, _currentUser);
            dashboard.Show();
            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            UserLoginView.Visibility = Visibility.Collapsed;
            UserScrollArea.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheck_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordEdit.Password;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordEdit.Visibility = Visibility.Collapsed;
        }

        private void ShowPasswordCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordEdit.Password = PasswordTextBox.Text;
            PasswordEdit.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
        }

        private static string GetSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static Color GetLighterColor(Color color, float factor)
        {
            return Color.FromArgb(color.A, (byte)(color.R * factor > 255 ? 255 : color.R * factor),
                                           (byte)(color.G * factor > 255 ? 255 : color.G * factor),
                                           (byte)(color.B * factor > 255 ? 255 : color.B * factor));
        }
    }
}
