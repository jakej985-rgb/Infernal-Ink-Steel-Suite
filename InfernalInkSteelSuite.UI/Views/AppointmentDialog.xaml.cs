using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Text.Json;
using System.IO;

namespace InfernalInkSteelSuite.UI.Views
{
    public partial class AppointmentDialog : Window, INotifyPropertyChanged
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ShopSettingsRepository _shopSettingsRepository;
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

        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    OnPropertyChanged(nameof(SelectedClient));
                    if (_selectedClient != null)
                    {
                        ClientSearchText = _selectedClient.FullName;
                        Appointment.ClientId = _selectedClient.Id;
                    }
                }
            }
        }

        public AppointmentDialog(IAppointmentRepository appointmentRepository, IClientRepository clientRepository)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _shopSettingsRepository = new ShopSettingsRepository(App.ConnectionString);

            _allClients = [.. _clientRepository.GetAll()];
            FilterClients(); // Initialize FilteredClients

            Appointment = new Appointment
            {
                // Default to today if new
                DateTime = DateTime.Today
            };

            InitializeCollections();
            InitializeTimeDefaults();

            DataContext = this;
        }

        public AppointmentDialog(IAppointmentRepository appointmentRepository, IClientRepository clientRepository, Appointment appointment)
        {
            InitializeComponent();
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _shopSettingsRepository = new ShopSettingsRepository(App.ConnectionString);

            _allClients = [.. _clientRepository.GetAll()];
            FilterClients();

            Appointment = appointment;

            if (Appointment.ClientId != 0)
            {
                var client = _allClients.FirstOrDefault(c => c.Id == Appointment.ClientId);
                if (client != null)
                {
                    SelectedClient = client;
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

        private void InitializeTimeDefaults()
        {
            SelectedHour = 12;
            SelectedMinute = "00";
            SelectedAmPm = "PM";
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

                // Validate Shop Hours
                var endTime = Appointment.DateTime.AddMinutes(Appointment.DurationMinutes);
                if (!ValidateShopHours(Appointment.DateTime, endTime))
                {
                    MessageBox.Show("Selected time is outside of shop hours.", "Shop Closed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check Conflicts
                if (CheckConflicts(Appointment.DateTime, endTime))
                {
                    MessageBox.Show("This time slot conflicts with another appointment or block-off.", "Conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Appointment.Id == 0)
                    _appointmentRepository.Add(Appointment);
                else
                    _appointmentRepository.Update(Appointment);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving appointment: {ex.Message}");
            }
        }
        private bool ValidateShopHours(DateTime start, DateTime end)
        {
            var settings = _shopSettingsRepository.LoadSettings();
            if (string.IsNullOrEmpty(settings.ShopHoursJson)) return true;

            List<ShopDaySetting>? shopHours;
            try
            {
                shopHours = JsonSerializer.Deserialize<List<ShopDaySetting>>(settings.ShopHoursJson);
            }
            catch
            {
                return true;
            }

            if (shopHours == null) return true;

            var daySetting = shopHours.FirstOrDefault(d => d.Day == start.DayOfWeek);
            if (daySetting == null) return true;

            if (!daySetting.IsOpen) return false;

            var shopStart = start.Date.Add(daySetting.StartTime);
            var shopEnd = start.Date.Add(daySetting.EndTime);

            return start >= shopStart && end <= shopEnd;
        }

        private bool CheckConflicts(DateTime start, DateTime end)
        {
            var appointments = _appointmentRepository.GetAppointmentsByDate(start.Date);
            foreach (var appt in appointments)
            {
                if (appt.Id == Appointment.Id) continue;

                var apptStart = appt.DateTime;
                var apptEnd = apptStart.AddMinutes(appt.DurationMinutes);

                if (start < apptEnd && end > apptStart)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
