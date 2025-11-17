using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.ViewModels
{
    public class QuotesViewModel : BaseViewModel
    {
        private readonly IAppointmentRepository _appointmentRepository;

        private decimal _averageTattooPrice;
        public decimal AverageTattooPrice
        {
            get => _averageTattooPrice;
            set
            {
                _averageTattooPrice = value;
                OnPropertyChanged();
            }
        }

        private decimal _averagePiercingPrice;
        public decimal AveragePiercingPrice
        {
            get => _averagePiercingPrice;
            set
            {
                _averagePiercingPrice = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<QuoteDto> Quotes { get; } = new ObservableCollection<QuoteDto>();

        public ICommand LoadQuotesCommand { get; }

        public QuotesViewModel(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
            LoadQuotesCommand = new RelayCommand(LoadQuotes);
            LoadQuotes();
        }

        private void LoadQuotes()
        {
            Quotes.Clear();
            var completedAppointments = _appointmentRepository.GetAppointmentsByStatus("complete");

            if (completedAppointments == null) return;

            foreach (var appointment in completedAppointments)
            {
                var pricePerHour = appointment.DurationMinutes > 0 ? (appointment.PriceCharged / (decimal)(appointment.DurationMinutes / 60.0)) : 0;
                Quotes.Add(new QuoteDto
                {
                    Service = appointment.ServiceType,
                    PriceCharged = appointment.PriceCharged,
                    DurationMinutes = appointment.DurationMinutes,
                    PricePerHour = pricePerHour
                });
            }

            AverageTattooPrice = CalculateAverage("tattoo", completedAppointments);
            AveragePiercingPrice = CalculateAverage("piercing", completedAppointments);
        }

        private decimal CalculateAverage(string serviceType, List<Appointment> appointments)
        {
            var filteredAppointments = appointments
                .Where(a => a.ServiceType != null && a.ServiceType.ToLower() == serviceType && a.DurationMinutes > 0);

            if (!filteredAppointments.Any())
            {
                return 0;
            }

            var total = filteredAppointments.Sum(a => a.PriceCharged / (decimal)(a.DurationMinutes / 60.0));
            return total / filteredAppointments.Count();
        }
    }

    public class QuoteDto
    {
        public string Service { get; set; }
        public decimal PriceCharged { get; set; }
        public int DurationMinutes { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
