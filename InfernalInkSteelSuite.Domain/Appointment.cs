using System;
using System.ComponentModel;

namespace InfernalInkSteelSuite.Domain
{
    public class Appointment : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private int _clientId;
        public int ClientId
        {
            get { return _clientId; }
            set { _clientId = value; OnPropertyChanged(nameof(ClientId)); }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
