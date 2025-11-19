using System;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Views.Appointments
{
    public partial class CalendarTab : UserControl
    {
        public CalendarTab()
        {
            InitializeComponent();
            AppointmentCalendar.SelectedDate = DateTime.Today;
        }

        private void AppointmentCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // This method is required by the XAML, but no longer has any logic.
        }
    }
}
