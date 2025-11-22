using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace InfernalInkSteelSuite.Views
{
    public partial class AppointmentDialog : Window, INotifyPropertyChanged
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        public Appointment Appointment { get; set; }
        public string Time { get; set; } = string.Empty;

        public ObservableCollection<string> ServiceTypes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ServiceCategories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> PriceTypes { get; set; } = new ObservableCollection<string>();

        private string _selectedServiceType = string.Empty;
        public string SelectedServiceType
        {
            get { return _selectedServiceType; }
            set
            {
                _selectedServiceType = value;
                Appointment.ServiceType = value;
                UpdateServiceCategories();
                OnPropertyChanged(nameof(SelectedServiceType));
            }
        }

        public AppointmentDialog(IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            ClientComboBox.ItemsSource = _clientRepository.GetAll();
            Appointment = new Appointment();
            InitializeCollections();
            DataContext = this;
        }

        public AppointmentDialog(IAppointmentRepository appointmentRepository, IClientRepository clientRepository, Appointment appointment)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            ClientComboBox.ItemsSource = _clientRepository.GetAll();
            Appointment = appointment;
            Time = appointment.DateTime.ToString("HH:mm");
            InitializeCollections();
            SelectedServiceType = Appointment.ServiceType; // Trigger category update

            // Ensure category is set after update if it matches
            if (ServiceCategories.Contains(Appointment.ServiceCategory))
            {
                // It's already bound, but just to be safe or if we need specific logic
            }
            DataContext = this;
        }

        private void InitializeCollections()
        {
            ServiceTypes = new ObservableCollection<string> { "Tattoo", "Piercing" };
            // ServiceCategories initialized empty, populated by selection
            PriceTypes = new ObservableCollection<string> { "Regular", "Promo" };
        }

        private void UpdateServiceCategories()
        {
            ServiceCategories.Clear();
            if (SelectedServiceType == "Tattoo")
            {
                ServiceCategories.Add("Touch-up");
                ServiceCategories.Add("Consultation");
                ServiceCategories.Add("Appointment");
            }
            else if (SelectedServiceType == "Piercing")
            {
                ServiceCategories.Add("Single");
                ServiceCategories.Add("Multi");
                ServiceCategories.Add("Jewelry");
            }

            // Reset category if it's not in the new list
            if (!ServiceCategories.Contains(Appointment.ServiceCategory))
            {
                Appointment.ServiceCategory = string.Empty;
                OnPropertyChanged(nameof(Appointment));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            if (!int.TryParse(Appointment.DurationMinutes.ToString(), out _))
            {
                MessageBox.Show("Please enter a valid duration.");
                return;
            }
            // Price Charged validation removed

            try
            {
                Appointment.DateTime = DatePicker.SelectedDate.Value.Date + timeOfDay;
                _appointmentRepository.Add(Appointment);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving appointment: {ex.Message}");
            }
        }
    }
}
