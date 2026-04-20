using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Views;
using InfernalInkSteelSuite.Views.Dashboard;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.ViewModels.Dashboard
{
    public class HomeDashboardViewModel : BaseViewModel
    {
        private readonly AppDbContext _db;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ClientRepository _clientRepository;
        private readonly ShopSettingsRepository _shopSettingsRepository;

        // Properties for Data Binding
        public string Greeting { get; private set; }
        public string UserName { get; private set; }
        public string UserInitials { get; private set; }
        public string UserRole { get; private set; }
        public string ShopName { get; private set; }
        public string CurrentDate { get; private set; }
        public string AppointmentSummary { get; private set; }
        public ObservableCollection<TodayAppointmentVm> TodayAppointments { get; set; }
        public ObservableCollection<DaySummaryVm> WeekDays { get; set; }
        public decimal TodayRevenue { get; set; }
        public int ThisWeekBookings { get; set; }
        public int NewClients { get; set; }
        public ObservableCollection<ActionItemVm> ActionInboxItems { get; set; }

        // Commands
        public ICommand SelectDayCommand { get; }
        public ICommand OpenAppointmentCommand { get; }
        public ICommand OpenClientCommand { get; }
        public ICommand CreateNewAppointmentCommand { get; }
        public ICommand CreateNewClientCommand { get; }
        public ICommand OpenDailySummaryCommand { get; }
        public ICommand OpenFullCalendarCommand { get; }
        public ICommand OpenClientsListCommand { get; }
        public ICommand OpenInventoryCommand { get; }
        public ICommand OpenShopSettingsCommand { get; }
        public ICommand ResolveActionItemCommand { get; }

        public HomeDashboardViewModel(User currentUser, AppDbContext db)
        {
            _db = db;
            _shopSettingsRepository = new ShopSettingsRepository(db);
            _appointmentRepository = new AppointmentRepository(db);
            _clientRepository = new ClientRepository(db);

            // Initialize Collections
            TodayAppointments = [];
            WeekDays = [];
            ActionInboxItems = [];

            // Set Dynamic Properties
            UserName = currentUser.Username ?? "User";
            UserInitials = string.IsNullOrEmpty(UserName) ? "?" : UserName[..1].ToUpper();
            UserRole = currentUser.Role ?? "Guest";
            var shopSettings = _shopSettingsRepository.LoadSettings();
            ShopName = string.IsNullOrEmpty(shopSettings.ShopName) ? "Infernal Ink & Steel" : shopSettings.ShopName;
            CurrentDate = DateTime.Now.ToString("ddd, MMM dd, yyyy");
            Greeting = string.Empty;
            AppointmentSummary = string.Empty;
            SetGreeting();


            LoadDashboardData();

            // Initialize Commands
            SelectDayCommand = new RelayCommand(SelectDay);
            OpenAppointmentCommand = new RelayCommand(p => Console.WriteLine("Open Appointment"));
            OpenClientCommand = new RelayCommand(p => Console.WriteLine("Open Client"));

            CreateNewAppointmentCommand = new RelayCommand(p =>
            {
                var appointment = new Appointment { DateTime = DateTime.Today };
                var dialog = new AppointmentDialog(_db, appointment)
                {
                    Owner = Application.Current.MainWindow
                };
                if (dialog.ShowDialog() == true)
                {
                    // Refresh logic if needed
                }
            });

            CreateNewClientCommand = new RelayCommand(p =>
            {
                var vm = new AddEditClientViewModel(_clientRepository, new Client());
                var view = new AddEditClientView(vm)
                {
                    Owner = Application.Current.MainWindow
                };
                // Assuming we want to show it as a dialog
                view.ShowDialog();
            });

            OpenDailySummaryCommand = new RelayCommand(p =>
            {
                var vm = new DailySummaryViewModel(_appointmentRepository);
                var view = new DailySummaryView(vm)
                {
                    Owner = Application.Current.MainWindow
                };
                view.ShowDialog();
            });

            OpenFullCalendarCommand = new RelayCommand(p => (Application.Current.MainWindow as DashboardWindow)?.Appointments_Click(null, null));
            OpenClientsListCommand = new RelayCommand(p => (Application.Current.MainWindow as DashboardWindow)?.Clients_Click(null, null));
            OpenInventoryCommand = new RelayCommand(p => MessageBox.Show("Inventory management coming soon!", "Information", MessageBoxButton.OK, MessageBoxImage.Information));
            OpenShopSettingsCommand = new RelayCommand(p => (Application.Current.MainWindow as DashboardWindow)?.Settings_Click(null, null));
            ResolveActionItemCommand = new RelayCommand(p => MessageBox.Show("Action item resolution implemented in future update.", "Information", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void SetGreeting()
        {
            var hour = DateTime.Now.Hour;
            if (hour < 12)
                Greeting = $"Good morning, {UserName}";
            else if (hour < 18)
                Greeting = $"Good afternoon, {UserName}";
            else
                Greeting = $"Good evening, {UserName}";
        }

        private void PopulateWeekDays()
        {
            WeekDays.Clear();
            var today = DateTime.Now;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                WeekDays.Add(new()
                {
                    Date = date,
                    DayLabel = date.ToString("ddd"),
                    DateNumber = date.Day,
                    IsSelected = date.Date == today.Date
                });
            }
        }

        private void SelectDay(object? parameter)
        {
            if (parameter is not DaySummaryVm selectedDay)
            {
                return;
            }

            foreach (var day in WeekDays)
            {
                day.IsSelected = false;
            }
            selectedDay.IsSelected = true;

            LoadAppointmentsForDate(selectedDay.Date);
        }

        private void LoadDashboardData()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            // Today's Stats
            var todayAppts = _appointmentRepository.GetAppointmentsByDateRange(today, today.AddDays(1).AddSeconds(-1));
            TodayRevenue = todayAppts
                .Where(a => a.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) || a.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                .Sum(a => a.PriceCharged);

            // Weekly Stats
            var weekAppts = _appointmentRepository.GetAppointmentsByDateRange(startOfWeek, endOfWeek);
            ThisWeekBookings = weekAppts.Count;

            // Client Growth
            var allClients = _clientRepository.GetAll();
            NewClients = allClients.Count(c => c.CreatedAt >= startOfWeek && c.CreatedAt < endOfWeek);

            LoadAppointmentsForDate(today);
            PopulateWeekDays();

            // Action Items (Simple logic)
            ActionInboxItems.Clear();
            var noShows = todayAppts.Where(a => a.Status.Equals("No-Show", StringComparison.OrdinalIgnoreCase));
            foreach (var ns in noShows)
            {
                ActionInboxItems.Add(new ActionItemVm { Icon = "⚠", Description = $"Follow up on No-Show: {ns.Client?.FirstName ?? "Unknown"}" });
            }

            if (!ActionInboxItems.Any())
            {
                ActionInboxItems.Add(new ActionItemVm { Icon = "✅", Description = "All caught up! No urgent actions." });
            }
        }

        private void LoadAppointmentsForDate(DateTime date)
        {
            var appts = _appointmentRepository.GetAppointmentsByDateRange(date.Date, date.Date.AddDays(1).AddSeconds(-1));
            TodayAppointments.Clear();
            foreach (var a in appts)
            {
                TodayAppointments.Add(new TodayAppointmentVm
                {
                    TimeRange = $"{a.StartTime:HH:mm}–{a.EndTime:HH:mm}",
                    ClientName = $"{a.Client?.FirstName} {a.Client?.LastName}".Trim(),
                    Service = a.ServiceType,
                    Artist = a.Artist?.Username?[..Math.Min(2, a.Artist.Username.Length)].ToUpper() ?? "??",
                    Status = a.Status
                });
            }

            AppointmentSummary = $"{TodayAppointments.Count} booked · {appts.Count(a => a.IsBlockOff)} blocked";
            OnPropertyChanged(nameof(AppointmentSummary));
            OnPropertyChanged(nameof(TodayRevenue));
            OnPropertyChanged(nameof(ThisWeekBookings));
            OnPropertyChanged(nameof(NewClients));
        }
    }
}
