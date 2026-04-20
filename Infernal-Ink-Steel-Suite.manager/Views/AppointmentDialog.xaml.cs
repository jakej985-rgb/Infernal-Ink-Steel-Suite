using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Text.Json;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.Views
{
    public partial class AppointmentDialog : Window, INotifyPropertyChanged
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ShopSettingsRepository _shopSettingsRepository;

        public ClientSearchViewModel ClientSearch { get; }
        public TimePickerViewModel TimePicker { get; }

        public Appointment Appointment { get; set; }

        public ObservableCollection<string> ServiceTypes { get; set; } = [];
        public ObservableCollection<string> ServiceCategories { get; set; } = [];
        public ObservableCollection<string> PriceTypes { get; set; } = [];

        private string _selectedServiceType = string.Empty;
        public string SelectedServiceType
        {
            get => _selectedServiceType;
            set
            {
                _selectedServiceType = value;
                Appointment.ServiceType = value;
                UpdateServiceCategories();
                OnPropertyChanged(nameof(SelectedServiceType));
            }
        }

        public AppointmentDialog(AppDbContext db)
        {
            _appointmentRepository = new AppointmentRepository(db);
            _clientRepository = new ClientRepository(db);
            _shopSettingsRepository = new ShopSettingsRepository(db);

            ClientSearch = new ClientSearchViewModel(_clientRepository);
            TimePicker = new TimePickerViewModel();

            InitializeComponent();

            Appointment = new Appointment { DateTime = DateTime.Today };
            InitializeCollections();
            DataContext = this;
        }

        public AppointmentDialog(AppDbContext db, Appointment appointment)
        {
            _appointmentRepository = new AppointmentRepository(db);
            _clientRepository = new ClientRepository(db);
            _shopSettingsRepository = new ShopSettingsRepository(db);

            ClientSearch = new ClientSearchViewModel(_clientRepository);
            TimePicker = new TimePickerViewModel();

            InitializeComponent();

            Appointment = appointment;
            if (Appointment.ClientId != 0) ClientSearch.SelectClient(Appointment.ClientId);

            InitializeCollections();
            TimePicker.LoadFromDateTime(Appointment.DateTime);

            SelectedServiceType = Appointment.ServiceType ?? string.Empty;
            DataContext = this;
        }

        private void InitializeCollections()
        {
            ServiceTypes = ["Tattoo", "Piercing"];
            PriceTypes = ["Regular", "Promo"];
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

            if (!ServiceCategories.Contains(Appointment.ServiceCategory))
            {
                Appointment.ServiceCategory = string.Empty;
                OnPropertyChanged(nameof(Appointment));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientSearch.SelectedClient == null)
            {
                MessageBox.Show("Please select a client.");
                return;
            }
            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.");
                return;
            }

            try
            {
                Appointment.ClientId = ClientSearch.SelectedClient.Id;
                Appointment.DateTime = TimePicker.ApplyToDate(DatePicker.SelectedDate.Value);

                var endTime = Appointment.DateTime.AddMinutes(Appointment.DurationMinutes);
                if (!ValidateShopHours(Appointment.DateTime, endTime))
                {
                    MessageBox.Show("Selected time is outside of shop hours.", "Shop Closed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CheckConflicts(Appointment.DateTime, endTime))
                {
                    MessageBox.Show("This time slot conflicts with another appointment.", "Conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Appointment.Id == 0) _appointmentRepository.Add(Appointment);
                else _appointmentRepository.Update(Appointment);

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
            try
            {
                var shopHours = JsonSerializer.Deserialize<List<ShopDaySetting>>(settings.ShopHoursJson);
                var daySetting = shopHours?.FirstOrDefault(d => d.Day == start.DayOfWeek);
                if (daySetting == null || !daySetting.IsOpen) return false;
                return start.TimeOfDay >= daySetting.StartTime && end.TimeOfDay <= daySetting.EndTime;
            }
            catch { return true; }
        }

        private bool CheckConflicts(DateTime start, DateTime end)
        {
            var appointments = _appointmentRepository.GetAppointmentsByDate(start.Date);
            return appointments.Any(appt => appt.Id != Appointment.Id && start < appt.DateTime.AddMinutes(appt.DurationMinutes) && end > appt.DateTime);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
