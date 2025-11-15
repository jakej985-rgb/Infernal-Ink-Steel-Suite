using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Windows;
using System.Windows.Controls;

namespace Infernal_Ink_Steel_Suite.manager
{
    public partial class LoginPage : Page
    {
        private readonly UserRepository _userRepository;

        public LoginPage()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var user = _userRepository.GetUserByUsername(UsernameTextBox.Text);
            if (user != null && user.Password == PasswordBox.Password)
            {
                // Navigate to the main application window
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Window.GetWindow(this).Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }
    }
}
