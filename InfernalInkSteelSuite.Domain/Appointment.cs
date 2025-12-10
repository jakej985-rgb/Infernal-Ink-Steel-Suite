using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfernalInkSteelSuite.Domain
{
    public class Appointment : INotifyPropertyChanged, ISyncEntity
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        // Sync Properties
        public Guid SyncId { get; set; } = Guid.NewGuid();
        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
        public string LastModifiedBy { get; set; } = "";
        public bool IsDeleted { get; set; }
        public byte[]? RowVersion { get; set; }

        private int _clientId;
        public int ClientId
        {
            get { return _clientId; }
            set { _clientId = value; OnPropertyChanged(nameof(ClientId)); }
        }

        [NotMapped]
        public Guid? ClientSyncId { get; set; }

        private int _userId;
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; OnPropertyChanged(nameof(UserId)); }
        }

        private DateTime _dateTime;
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; OnPropertyChanged(nameof(DateTime)); }
        }

        private int _durationMinutes;
        public int DurationMinutes
        {
            get { return _durationMinutes; }
            set { _durationMinutes = value; OnPropertyChanged(nameof(DurationMinutes)); }
        }

        private string _serviceType = "";
        public string ServiceType
        {
            get { return _serviceType; }
            set { _serviceType = value; OnPropertyChanged(nameof(ServiceType)); }
        }

        private string _serviceCategory = "";
        public string ServiceCategory
        {
            get { return _serviceCategory; }
            set { _serviceCategory = value; OnPropertyChanged(nameof(ServiceCategory)); }
        }

        private string _priceType = "";
        public string PriceType
        {
            get { return _priceType; }
            set { _priceType = value; OnPropertyChanged(nameof(PriceType)); }
        }

        private decimal _priceCharged;
        public decimal PriceCharged
        {
            get { return _priceCharged; }
            set { _priceCharged = value; OnPropertyChanged(nameof(PriceCharged)); }
        }

        private decimal? _quotedPrice;
        public decimal? QuotedPrice
        {
            get { return _quotedPrice; }
            set { _quotedPrice = value; OnPropertyChanged(nameof(QuotedPrice)); }
        }

        private decimal? _finalPrice;
        public decimal? FinalPrice
        {
            get { return _finalPrice; }
            set { _finalPrice = value; OnPropertyChanged(nameof(FinalPrice)); }
        }

        private string _notes = "";
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; OnPropertyChanged(nameof(Notes)); }
        }

        private string _clientName = "";
        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; OnPropertyChanged(nameof(ClientName)); }
        }

        private string _color = "";
        public string Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged(nameof(Color)); }
        }

        private string _status = "Scheduled";
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        private bool _isBlockOff;
        public bool IsBlockOff
        {
            get { return _isBlockOff; }
            set { _isBlockOff = value; OnPropertyChanged(nameof(IsBlockOff)); }
        }

        // Compatibility properties for API
        [NotMapped]
        public DateTime StartTime
        {
            get => DateTime;
            set => DateTime = value;
        }

        [NotMapped]
        public DateTime EndTime
        {
            get => DateTime.AddMinutes(DurationMinutes);
            set => DurationMinutes = (int)(value - DateTime).TotalMinutes;
        }

        [NotMapped]
        public int ArtistId
        {
            get => UserId;
            set => UserId = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Client? Client { get; set; }
        public virtual User? Artist { get; set; }
    }
}
