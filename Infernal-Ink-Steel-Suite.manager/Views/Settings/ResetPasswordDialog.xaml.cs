using System.Windows;

namespace Infernal_Ink_Steel_Suite.manager.Views.Settings
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
