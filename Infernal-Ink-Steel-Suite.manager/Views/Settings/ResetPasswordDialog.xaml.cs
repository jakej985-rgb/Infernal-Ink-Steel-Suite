using System.Windows;

namespace InfernalInkSteelSuite.Views.Settings
{
    public partial class ResetPasswordDialog : Window
    {
        public ResetPasswordDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
