using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Views.Appointments;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InfernalInkSteelSuite.Views
{
    public partial class AppointmentsView : UserControl
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;

        public AppointmentsView(IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;

            UpcomingTab.Content = new UpcomingAppointmentsTab(_appointmentRepository);
            PendingTab.Content = new PendingAppointmentsTab(_appointmentRepository);
            CompletedTab.Content = new CompletedAppointmentsTab(_appointmentRepository);
            CalendarTab.Content = new CalendarTab();

            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            RefreshButton.Click += RefreshButton_Click;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AppointmentDialog(_appointmentRepository, _clientRepository);
            if (dialog.ShowDialog() == true)
            {
                RefreshAppointments();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MainTabWidget.SelectedItem as TabItem;
            if (selectedTab?.Content is IAppointmentTabView tabView)
            {
                var selectedAppointment = tabView.SelectedAppointment;
                if (selectedAppointment != null)
                {
                    var dialog = new EditAppointmentDialog(selectedAppointment, _appointmentRepository, _clientRepository);
                    if (dialog.ShowDialog() == true)
                    {
                        RefreshAppointments();
                    }
                }
                else
                {
                    MessageBox.Show("Please select an appointment to edit.");
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MainTabWidget.SelectedItem as TabItem;
            if (selectedTab?.Content is IAppointmentTabView tabView)
            {
                var selectedAppointment = tabView.SelectedAppointment;
                if (selectedAppointment != null)
                {
                    if (MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _appointmentRepository.Delete(selectedAppointment.Id);
                        RefreshAppointments();
                    }
                }
                else
                {
                    MessageBox.Show("Please select an appointment to delete.");
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAppointments();
        }

        private void RefreshAppointments()
        {
            (UpcomingTab.Content as UpcomingAppointmentsTab)?.Refresh();
            (PendingTab.Content as PendingAppointmentsTab)?.Refresh();
            (CompletedTab.Content as CompletedAppointmentsTab)?.Refresh();
        }

        private void MainTabWidget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var isCalendarTab = MainTabWidget.SelectedItem == CalendarTab;
            EditButton.IsEnabled = !isCalendarTab;
            DeleteButton.IsEnabled = !isCalendarTab;
        }
    }
}
