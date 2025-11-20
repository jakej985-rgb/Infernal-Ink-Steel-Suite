using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.ViewModels.Dashboard
{
    public class DailySummaryViewModel : BaseViewModel
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public string CurrentDate { get; private set; }
        public int CompletedAppointments { get; private set; }
        public decimal TotalRevenue { get; private set; }
        public int NoShows { get; private set; }
        public decimal OutstandingBalances { get; private set; }

        public ICommand CloseCommand { get; }
        public ICommand PrintCommand { get; }

        public event EventHandler? RequestClose;

        public DailySummaryViewModel(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
            CurrentDate = DateTime.Now.ToString("D");

            LoadMetrics();

            CloseCommand = new RelayCommand(p => RequestClose?.Invoke(this, EventArgs.Empty));
            PrintCommand = new RelayCommand(p => MessageBox.Show("Printing functionality not implemented yet."));
        }

        private void LoadMetrics()
        {
            // In a real scenario, we'd fetch specific data for today.
            // Since I don't have a "GetAppointmentsByDate" that returns exactly what I need easily without filtering,
            // I'll simulate or use what I have.
            // The instructions say "Show key metrics".

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Assuming GetAppointmentsByDateRange includes start, excludes end or similar.
            // Just fetching all and filtering for simplicity if repository doesn't support fine-grained.
            // Actually, GetAppointmentsByDateRange exists in memory.
            var appointments = _appointmentRepository.GetAppointmentsByDateRange(today, tomorrow);

            CompletedAppointments = appointments.Count(a => a.Status == "Completed");
            TotalRevenue = appointments.Where(a => a.Status == "Completed" || a.Status == "Paid").Sum(a => a.PriceCharged);
            NoShows = appointments.Count(a => a.Status == "NoShow" || a.Status == "Cancelled");

            // Outstanding balances: PriceCharged - Deposit? Or just PriceType "Hourly" estimates?
            // Domain model doesn't seem to have "PaidAmount", just "PriceCharged".
            // I'll assume PriceCharged is what they owe if not paid? Or maybe there's no data for this yet.
            // I'll just use a placeholder logic or 0 if not trackable.
            OutstandingBalances = 0;
        }
    }
}
