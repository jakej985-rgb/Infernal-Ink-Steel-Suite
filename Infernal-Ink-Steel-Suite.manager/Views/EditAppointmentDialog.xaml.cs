using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Windows;

namespace InfernalInkSteelSuite.Views
{
    public partial class EditAppointmentDialog : Window
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        public Appointment Appointment { get; set; }
        public string Time { get; set; }

        public EditAppointmentDialog(Appointment appointment, IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            ClientComboBox.ItemsSource = _clientRepository.GetAll();

            Appointment = appointment;
            Time = Appointment.DateTime.ToString("HH:mm");
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a client.");
                return;
            }
            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.");
                return;
            }
            if (!TimeSpan.TryParse(Time, out var timeOfDay))
            {
                MessageBox.Show("Please enter a valid time.");
                return;
            }
            if (!int.TryParse(DurationTextBox.Text, out _))
            {
                MessageBox.Show("Please enter a valid duration.");
                return;
            }
            if (!decimal.TryParse(PriceChargedTextBox.Text, out _))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            try
            {
                Appointment.DateTime = DatePicker.SelectedDate.Value.Date + timeOfDay;
                _appointmentRepository.Update(Appointment);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving appointment: {ex.Message}");
            }
        }
    }
}
