using InfernalInkSteelSuite.Domain;

namespace Infernal_Ink_Steel_Suite.manager.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        private readonly Document _document;
        private string _clientName;

        public DocumentViewModel(Document document)
        {
            _document = document;
        }

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
