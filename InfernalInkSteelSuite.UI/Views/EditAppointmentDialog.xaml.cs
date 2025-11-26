using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace InfernalInkSteelSuite.UI.Views
{
    public partial class EditAppointmentDialog : Window, INotifyPropertyChanged
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly List<Client> _allClients;

        public Appointment Appointment { get; set; }

        public ObservableCollection<string> ServiceTypes { get; set; } = [];
        public ObservableCollection<string> ServiceCategories { get; set; } = [];
        public ObservableCollection<string> PriceTypes { get; set; } = [];

        // Time Properties
        public ObservableCollection<int> Hours { get; set; } = [];
        public ObservableCollection<string> Minutes { get; set; } = [];
        public ObservableCollection<string> AmPmOptions { get; set; } = ["AM", "PM"];

        private int _selectedHour;
        public int SelectedHour
        {
            get => _selectedHour;
            set { _selectedHour = value; OnPropertyChanged(nameof(SelectedHour)); }
        }

        private string _selectedMinute = "00";
        public string SelectedMinute
        {
            get => _selectedMinute;
            set { _selectedMinute = value; OnPropertyChanged(nameof(SelectedMinute)); }
        }

        private string _selectedAmPm = "AM";
        public string SelectedAmPm
        {
            get => _selectedAmPm;
            set { _selectedAmPm = value; OnPropertyChanged(nameof(SelectedAmPm)); }
        }

        // Client Search Properties
        private string _clientSearchText = string.Empty;
        public string ClientSearchText
        {
            get => _clientSearchText;
            set
            {
                _clientSearchText = value;
                OnPropertyChanged(nameof(ClientSearchText));
                FilterClients();
            }
        }

        public ObservableCollection<Client> FilteredClients { get; set; } = [];

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

        public EditAppointmentDialog(Appointment appointment, IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;

            _allClients = [.. _clientRepository.GetAll()];
            FilterClients();

            Appointment = appointment;

            if (Appointment.ClientId != 0)
            {
                var client = _allClients.FirstOrDefault(c => c.Id == Appointment.ClientId);
                if (client != null)
                {
                    ClientSearchText = client.FullName;
                }
            }

            InitializeCollections();
            InitializeTimeFromAppointment();

            SelectedServiceType = Appointment.ServiceType; // Trigger category update

            // Ensure category is set after update if it matches
            if (ServiceCategories.Contains(Appointment.ServiceCategory))
            {
                // It's already bound
            }
            else
            {
                Appointment.ServiceCategory = string.Empty;
            }
            DataContext = this;
        }

        private void InitializeCollections()
        {
            ServiceTypes = ["Tattoo", "Piercing"];
            PriceTypes = ["Regular", "Promo"];

            for (int i = 1; i <= 12; i++) Hours.Add(i);
            Minutes.Clear();
            for (int i = 0; i < 60; i += 5) Minutes.Add(i.ToString("D2"));
        }

        private void InitializeTimeFromAppointment()
        {
            var time = Appointment.DateTime;
            int hour = time.Hour;
            SelectedAmPm = hour >= 12 ? "PM" : "AM";

            if (hour == 0) SelectedHour = 12;
            else if (hour > 12) SelectedHour = hour - 12;
            else SelectedHour = hour;

            string minStr = time.Minute.ToString("D2");
            if (!Minutes.Contains(minStr))
            {
                Minutes.Add(minStr);
                var sorted = Minutes.OrderBy(x => x).ToList();
                Minutes.Clear();
                foreach (var m in sorted) Minutes.Add(m);
            }
            SelectedMinute = minStr;
        }

        private void FilterClients()
        {
            FilteredClients.Clear();
            if (string.IsNullOrWhiteSpace(ClientSearchText))
            {
                foreach (var client in _allClients) FilteredClients.Add(client);
            }
            else
            {
                var search = ClientSearchText.ToLower();
                var matches = _allClients.Where(c =>
                    c.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));

                foreach (var client in matches) FilteredClients.Add(client);
            }
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
            if (Appointment.ClientId == 0)
            {
                MessageBox.Show("Please select a client.");
                return;
            }
            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.");
                return;
            }

            if (!int.TryParse(Appointment.DurationMinutes.ToString(), out _))
            {
                MessageBox.Show("Please enter a valid duration.");
                return;
            }

            try
            {
                // Construct Time
                int hour = SelectedHour;
                if (SelectedAmPm == "PM" && hour != 12) hour += 12;
                if (SelectedAmPm == "AM" && hour == 12) hour = 0;

                int minute = int.Parse(SelectedMinute);
                var timeOfDay = new TimeSpan(hour, minute, 0);

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
