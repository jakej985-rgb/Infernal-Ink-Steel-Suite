using System.Windows;
using InfernalInkSteelSuite.Views;

namespace InfernalInkSteelSuite
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            // TODO: swap this with real DB login later
            if (username == "admin" && password == "password")
            {
                var dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
            }
            else
            {
                StatusText.Text = "Invalid credentials.";
            }
        }
    }
}
