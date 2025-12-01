using System.ComponentModel;

namespace InfernalInkSteelSuite.Domain
{
    public class Client : INotifyPropertyChanged, ISyncEntity
    {
        private string _firstName = "";
        private string _lastName = "";
        private string _photoPath = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }

        // Sync Properties
        public Guid SyncId { get; set; } = Guid.NewGuid();
        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
        public string LastModifiedBy { get; set; } = "";
        public bool IsDeleted { get; set; }
        public byte[]? RowVersion { get; set; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string MiddleName { get; set; } = "";

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Notes { get; set; } = "";
        public int Visits { get; set; }

        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                _photoPath = value;
                OnPropertyChanged(nameof(PhotoPath));
            }
        }


        public string FullName
        {
            get
            {
                var nameParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    nameParts.Add(FirstName.Trim());
                }
                if (!string.IsNullOrWhiteSpace(MiddleName))
                {
                    nameParts.Add(MiddleName.Trim());
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    nameParts.Add(LastName.Trim());
                }
                return string.Join(" ", nameParts);
            }
        }

        public virtual ICollection<Appointment> Appointments { get; set; } = [];
        public virtual ICollection<Document> Documents { get; set; } = [];

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
