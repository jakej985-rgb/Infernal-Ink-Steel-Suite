using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Views;
using InfernalInkSteelSuite.Views.Dashboard;

namespace InfernalInkSteelSuite.ViewModels.Dashboard
{
    public class HomeDashboardViewModel : BaseViewModel
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;

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
        public ICommand LogoutCommand { get; }
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

        public HomeDashboardViewModel(User currentUser, IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
                                   ?? "Data Source=shop_manager.db";
            _appointmentRepository = new AppointmentRepository(connectionString);
            _clientRepository = new ClientRepository(connectionString);

            // Initialize Collections
            TodayAppointments = [];
            WeekDays = [];
            ActionInboxItems = [];

            // Set Dynamic Properties
            UserName = currentUser.Username ?? "User";
            UserInitials = string.IsNullOrEmpty(UserName) ? "?" : UserName[..1].ToUpper();
            UserRole = currentUser.Role ?? "Guest";
            var shopSettings = shopSettingsRepository.LoadSettings();
            ShopName = string.IsNullOrEmpty(shopSettings.ShopName) ? "Infernal Ink & Steel" : shopSettings.ShopName;
            CurrentDate = DateTime.Now.ToString("ddd, MMM dd, yyyy");
            Greeting = string.Empty;
            SetGreeting();


            // Mock Data
            TodayRevenue = 1250;
            ThisWeekBookings = 18;
            NewClients = 3;

            TodayAppointments =
            [
                new() { TimeRange = "11:00–12:30", ClientName = "Maria Lopez", Service = "Full sleeve linework", Artist = "AB", Status = "Confirmed" },
                new() { TimeRange = "13:00–14:00", ClientName = "John Smith", Service = "Piercing", Artist = "CD", Status = "Pending" }
            ];

            AppointmentSummary = $"{TodayAppointments.Count} booked · 1 no-show risk · 2 walk-ins";

            PopulateWeekDays();

            ActionInboxItems =
            [
                new() { Icon = "⚠", Description = "Unsigned consent form – Maria Lopez (Today 11:00)" },
                new() { Icon = "💰", Description = "Deposit overdue – John Smith (Tomorrow 14:00)" }
            ];

            // Initialize Commands
            SelectDayCommand = new RelayCommand(SelectDay);
            LogoutCommand = new RelayCommand(p =>
            {
                var login = new Login();
                login.Show();
                Application.Current.MainWindow.Close();
            });
            OpenAppointmentCommand = new RelayCommand(p => Console.WriteLine("Open Appointment"));
            OpenClientCommand = new RelayCommand(p => Console.WriteLine("Open Client"));

            CreateNewAppointmentCommand = new RelayCommand(p =>
            {
                var appointment = new Appointment { DateTime = DateTime.Today };
                var dialog = new AppointmentDialog(_appointmentRepository, _clientRepository, appointment);
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog() == true)
                {
                    // Refresh logic if needed
                }
            });

            CreateNewClientCommand = new RelayCommand(p =>
            {
                var vm = new AddEditClientViewModel(_clientRepository, new Client());
                var view = new AddEditClientView(vm);
                view.Owner = Application.Current.MainWindow;
                // Assuming we want to show it as a dialog
                view.ShowDialog();
            });

            OpenDailySummaryCommand = new RelayCommand(p =>
            {
                var vm = new DailySummaryViewModel(_appointmentRepository);
                var view = new DailySummaryView(vm);
                view.Owner = Application.Current.MainWindow;
                view.ShowDialog();
            });

            OpenFullCalendarCommand = new RelayCommand(p => Console.WriteLine("Open Full Calendar"));
            OpenClientsListCommand = new RelayCommand(p => Console.WriteLine("Open Clients List"));
            OpenInventoryCommand = new RelayCommand(p => Console.WriteLine("Open Inventory"));
            OpenShopSettingsCommand = new RelayCommand(p => Console.WriteLine("Open Shop Settings"));
            ResolveActionItemCommand = new RelayCommand(p => Console.WriteLine("Resolve Action Item"));
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
            var today = DateTime.Now;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                WeekDays.Add(new DaySummaryVm
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
            if (parameter is DaySummaryVm selectedDay)
            {
                foreach (var day in WeekDays)
                {
                    day.IsSelected = false;
                }
                selectedDay.IsSelected = true;

                // In a real implementation, you would load appointments for the selected day here.
                // For now, we'll just clear and add a dummy item to show it works.
                TodayAppointments.Clear();
                if (selectedDay.Date.Date == DateTime.Now.Date)
                {
                    TodayAppointments.Add(new() { TimeRange = "11:00–12:30", ClientName = "Maria Lopez", Service = "Full sleeve linework", Artist = "AB", Status = "Confirmed" });
                    TodayAppointments.Add(new() { TimeRange = "13:00–14:00", ClientName = "John Smith", Service = "Piercing", Artist = "CD", Status = "Pending" });
                }
                else
                {
                    TodayAppointments.Add(new() { TimeRange = "10:00-11:00", ClientName = "Another Client", Service = "Consultation", Artist = "XY", Status = "Confirmed"});
                }
            }
        }
    }
}
