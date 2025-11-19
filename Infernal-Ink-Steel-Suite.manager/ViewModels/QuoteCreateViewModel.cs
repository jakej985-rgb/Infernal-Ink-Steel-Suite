using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace InfernalInkSteelSuite.ViewModels
{
    public class QuoteCreateViewModel : BaseViewModel
    {
        private readonly ITattooPricingService _pricingService;
        private readonly IQuoteRepository _quoteRepository;
        private QuoteInput _quoteInput;
        private QuoteEstimate _quoteEstimate;

        // Repositories will be injected later
        // private readonly IClientRepository _clientRepository;
        // private readonly IUserRepository _userRepository;

        #region Input Properties

        public int? ClientId
        {
            get => _quoteInput.ClientId;
            set { if (_quoteInput.ClientId != value) { _quoteInput.ClientId = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public string Placement
        {
            get => _quoteInput.Placement;
            set { if (_quoteInput.Placement != value) { _quoteInput.Placement = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public string Style
        {
            get => _quoteInput.Style;
            set { if (_quoteInput.Style != value) { _quoteInput.Style = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public bool IsCoverUp
        {
            get => _quoteInput.IsCoverUp;
            set { if (_quoteInput.IsCoverUp != value) { _quoteInput.IsCoverUp = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public double Width
        {
            get => _quoteInput.Width;
            set { if (_quoteInput.Width != value) { _quoteInput.Width = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public double Height
        {
            get => _quoteInput.Height;
            set { if (_quoteInput.Height != value) { _quoteInput.Height = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public int CoverageLevel
        {
            get => _quoteInput.CoverageLevel;
            set { if (_quoteInput.CoverageLevel != value) { _quoteInput.CoverageLevel = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public int LineComplexity
        {
            get => _quoteInput.LineComplexity;
            set { if (_quoteInput.LineComplexity != value) { _quoteInput.LineComplexity = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public int ShadingComplexity
        {
            get => _quoteInput.ShadingComplexity;
            set { if (_quoteInput.ShadingComplexity != value) { _quoteInput.ShadingComplexity = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public int ColorComplexity
        {
            get => _quoteInput.ColorComplexity;
            set { if (_quoteInput.ColorComplexity != value) { _quoteInput.ColorComplexity = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public int Difficulty
        {
            get => _quoteInput.Difficulty;
            set { if (_quoteInput.Difficulty != value) { _quoteInput.Difficulty = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        public int ArtistId
        {
            get => _quoteInput.ArtistId;
            set { if (_quoteInput.ArtistId != value) { _quoteInput.ArtistId = value; OnPropertyChanged(); RecalculateEstimate(); } }
        }

        #endregion

        #region Estimate Properties

        public QuoteEstimate QuoteEstimate
        {
            get => _quoteEstimate;
            set
            {
                _quoteEstimate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstimatedHoursRange));
                OnPropertyChanged(nameof(EstimatedPriceRange));
            }
        }

        public string EstimatedHoursRange => $"{QuoteEstimate?.EstimatedHoursLow:F1} - {QuoteEstimate?.EstimatedHoursHigh:F1} hours";
        public string EstimatedPriceRange => $"{QuoteEstimate?.PriceLow:C} - {QuoteEstimate?.PriceHigh:C}";

        #endregion

        #region UI Collections

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<User> Artists { get; set; }
        public ObservableCollection<string> Placements { get; set; }
        public ObservableCollection<string> Styles { get; set; }

        #endregion

        public ICommand RecalculateCommand { get; }
        public ICommand SaveQuoteCommand { get; }

        public QuoteCreateViewModel(ITattooPricingService pricingService, IQuoteRepository quoteRepository)
        {
            _pricingService = pricingService;
            _quoteRepository = quoteRepository;

            _quoteInput = new QuoteInput
            {
                CoverageLevel = 3,
                LineComplexity = 3,
                ShadingComplexity = 3,
                ColorComplexity = 3,
                Difficulty = 3,
                Width = 10,
                Height = 10,
                Placement = "Forearm",
                Style = "Fine line"
            };

            // Initialize collections with placeholder data
            Clients = new ObservableCollection<Client> { new Client { Id = 1, FirstName = "John", LastName = "Doe" } };
            Artists = new ObservableCollection<User> { new User { Id = 1, Username = "Artist One" } };
            Placements = new ObservableCollection<string> { "Forearm", "Calf", "Ribs", "Hand", "Neck" };
            Styles = new ObservableCollection<string> { "Fine line", "Traditional", "Neo-trad", "Realism", "Color realism", "Blackwork" };

            RecalculateCommand = new RelayCommand(_ => RecalculateEstimate());
            SaveQuoteCommand = new RelayCommand(_ => SaveQuote());
            RecalculateEstimate();
        }

        private void RecalculateEstimate()
        {
            QuoteEstimate = _pricingService.GetEstimate(_quoteInput);
        }

        private void SaveQuote()
        {
            var quote = new Quote
            {
                ClientId = _quoteInput.ClientId,
                ArtistId = _quoteInput.ArtistId,
                Placement = _quoteInput.Placement,
                Style = _quoteInput.Style,
                IsCoverUp = _quoteInput.IsCoverUp,
                Width = _quoteInput.Width,
                Height = _quoteInput.Height,
                CoverageLevel = _quoteInput.CoverageLevel,
                LineComplexity = _quoteInput.LineComplexity,
                ShadingComplexity = _quoteInput.ShadingComplexity,
                ColorComplexity = _quoteInput.ColorComplexity,
                Difficulty = _quoteInput.Difficulty,
                EstimatedHoursLow = _quoteEstimate.EstimatedHoursLow,
                EstimatedHoursHigh = _quoteEstimate.EstimatedHoursHigh,
                PriceLow = _quoteEstimate.PriceLow,
                PriceHigh = _quoteEstimate.PriceHigh,
                ShopMinimum = _quoteEstimate.ShopMinimum,
                RecommendedDeposit = _quoteEstimate.RecommendedDeposit,
                ConfidenceScore = _quoteEstimate.ConfidenceScore,
                SimilarJobsCount = _quoteEstimate.SimilarJobsCount,
                CreatedAt = System.DateTime.UtcNow
            };
            _quoteRepository.AddQuote(quote);
        }
    }
}
