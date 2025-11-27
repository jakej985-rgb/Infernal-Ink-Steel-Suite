using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.UI.ViewModels
{
    public class DocumentViewModel(Document document) : BaseViewModel
    {
        private readonly Document _document = document;
        private string _clientName = string.Empty;

        public int Id => _document.Id;
        public string Title => _document.Title;
        public System.DateTime CreatedAt => _document.CreatedAt;

        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value;
                OnPropertyChanged();
            }
        }
    }
}
