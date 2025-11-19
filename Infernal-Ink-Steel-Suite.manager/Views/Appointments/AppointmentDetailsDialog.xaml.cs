using System.Windows;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class AppointmentDetailsDialog : Window
    {
        public AppointmentDetailsDialog()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
