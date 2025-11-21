using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InfernalInkSteelSuite.Controls;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Views;

namespace InfernalInkSteelSuite
{
    public partial class Login : Window
    {
        private readonly UserRepository _userRepository;
        private readonly ShopSettingsRepository _settingsRepository;
        private User _currentUser;
        private readonly string _connectionString;

        public Login()
        {
            InitializeComponent();
            _connectionString = App.ConnectionString;
            _currentUser = null!;
            _userRepository = new UserRepository(_connectionString);
            _settingsRepository = new ShopSettingsRepository(_connectionString);
            BuildUserGrid();
            ApplyBranding();
        }

        private void BuildUserGrid()
        {
            UserGrid.Children.Clear();
            var users = _userRepository.GetAllUsers();
            foreach (var user in users)
            {
                var userWidget = new StackPanel { Margin = new Thickness(12) };

                var avatarGrid = new Grid
                {
                    Width = 80,
                    Height = 80,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var circle = new Ellipse();
                circle.SetResourceReference(Shape.FillBrushProperty, "AccentBrush");

                var initials = new TextBlock
                {
                    Text = user.Username[..1].ToUpper(),
                    FontSize = 36,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                initials.SetResourceReference(ForegroundProperty, "PrimaryTextBrush");

                avatarGrid.Children.Add(circle);
                avatarGrid.Children.Add(initials);

                var nameLabel = new TextBlock
                {
                    Text = user.Username,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 8, 0, 0)
                };
                nameLabel.SetResourceReference(ForegroundProperty, "PrimaryTextBrush");

                userWidget.Children.Add(avatarGrid);
                userWidget.Children.Add(nameLabel);
                userWidget.MouseDown += (sender, e) => ShowUserSelected(user);
                UserGrid.Children.Add(userWidget);
            }
        }

        private void ApplyBranding()
        {
            var settings = _settingsRepository.LoadSettings();
            const string defaultImagePath = "default_art.png";

            Title = string.IsNullOrWhiteSpace(settings.ShopName) ? "Login" : $"{settings.ShopName} Login";

            // Headline
            var shopName = string.IsNullOrWhiteSpace(settings.ShopName) ? "Infernal Ink & Steel" : settings.ShopName;
            HeadlineLabel.Text = $"Welcome \"{shopName}\"";

            // Special Message (Tagline)
            if (settings.IsSpecialMessageEnabled)
            {
                TaglineLabel.Text = string.IsNullOrWhiteSpace(settings.SpecialMessageText) ? "Sign In To Continue" : settings.SpecialMessageText;
                TaglineLabel.Visibility = Visibility.Visible;
            }
            else
            {
                TaglineLabel.Visibility = Visibility.Collapsed;
            }

            // Background Image
            if (!string.IsNullOrWhiteSpace(settings.LoginBackgroundPath) && File.Exists(settings.LoginBackgroundPath))
            {
                BackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(settings.LoginBackgroundPath)));
            }
            else if (File.Exists(defaultImagePath))
            {
                BackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(defaultImagePath)));
            }

            if (!string.IsNullOrWhiteSpace(settings.AccentColor))
            {
                var accentColor = (Color)ColorConverter.ConvertFromString(settings.AccentColor);
                var gradient = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                };
                gradient.GradientStops.Add(new GradientStop(GetLighterColor(accentColor, 1.3f), 0.0));
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
            AvatarInitials.Text = user.Username[..1].ToUpper();
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

            // Apply the user's theme BEFORE showing main window
            ThemeManager.ApplyTheme(_currentUser.ThemeKey);

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
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        private static Color GetLighterColor(Color color, float factor)
        {
            return Color.FromArgb(color.A, (byte)(color.R * factor > 255 ? 255 : color.R * factor),
                                           (byte)(color.G * factor > 255 ? 255 : color.G * factor),
                                           (byte)(color.B * factor > 255 ? 255 : color.B * factor));
        }
    }
}
