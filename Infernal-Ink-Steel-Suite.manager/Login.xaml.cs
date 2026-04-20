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
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite
{
    public partial class Login : Window
    {
        private readonly UserRepository _userRepository;
        private readonly ShopSettingsRepository _settingsRepository;
        private User _currentUser;

        public Login()
        {
            InitializeComponent();
            _currentUser = null!;
            if (App.LocalDb != null)
            {
                var hasher = new InfernalInkSteelSuite.Repositories.Services.PasswordHasher();
                _userRepository = new UserRepository(App.LocalDb, hasher);
                _settingsRepository = new ShopSettingsRepository(App.LocalDb);
            }
            else
            {
                // Fallback for design-time or error states
                throw new InvalidOperationException("Database not initialized.");
            }
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
                circle.SetResourceReference(Shape.FillProperty, "AccentBrush");

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
                BackgroundImage.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(settings.LoginBackgroundPath)));
            }
            else if (File.Exists(defaultImagePath))
            {
                BackgroundImage.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(defaultImagePath)));
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
                // Border brush styling removed - handled by control style
            }
        }

        private void ShowUserSelected(User user)
        {
            _currentUser = user;
            AvatarInitials.Text = user.Username[..1].ToUpper();
            SelectedUserLabel.Text = user.Username;
            PasswordEdit.Password = string.Empty;
            UserScrollArea.Visibility = Visibility.Collapsed;
            UserLoginView.Visibility = Visibility.Visible;
            PasswordEdit.FocusInput();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var password = PasswordEdit.Password;
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

            if (App.LocalDb == null) return;
            var dashboard = new DashboardWindow(App.LocalDb, _currentUser);
            dashboard.Show();
            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            UserLoginView.Visibility = Visibility.Collapsed;
            UserScrollArea.Visibility = Visibility.Visible;
        }

        private void UserLoginView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SignIn_Click(sender, e);
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
