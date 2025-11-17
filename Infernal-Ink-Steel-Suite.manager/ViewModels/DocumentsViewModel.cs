using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace InfernalInkSteelSuite.ViewModels
{
    public class DocumentsViewModel : BaseViewModel
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IClientRepository _clientRepository;
        private string _filterText;

        public ObservableCollection<DocumentViewModel> Documents { get; set; }
        public ICommand AddDocumentCommand { get; }
        public ICommand RefreshDocumentsCommand { get; }
        public ICommand FilterDocumentsCommand { get; }
        public ICommand OpenDocumentCommand { get; }
        public ICommand RenameDocumentCommand { get; }
        public ICommand DeleteDocumentCommand { get; }

        private DocumentViewModel _selectedDocument;
        public DocumentViewModel SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                _selectedDocument = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
            }
        }

        public DocumentsViewModel(IDocumentRepository documentRepository, IClientRepository clientRepository)
        {
            _documentRepository = documentRepository;
            _clientRepository = clientRepository;
            _selectedDocument = null!;
            _filterText = string.Empty;

            Documents = new ObservableCollection<DocumentViewModel>();

            AddDocumentCommand = new RelayCommand(_ => AddDocument());
            RefreshDocumentsCommand = new RelayCommand(_ => LoadDocuments());
            FilterDocumentsCommand = new RelayCommand(_ => FilterDocuments());
            OpenDocumentCommand = new RelayCommand(_ => OpenDocument(), _ => SelectedDocument != null);
            RenameDocumentCommand = new RelayCommand(_ => RenameDocument(), _ => SelectedDocument != null);
            DeleteDocumentCommand = new RelayCommand(_ => DeleteDocument(), _ => SelectedDocument != null);

            LoadDocuments();
        }

        private void OpenDocument()
        {
            if (SelectedDocument != null)
            {
                var document = _documentRepository.Get(SelectedDocument.Id);
                if (document != null)
                {
                    System.Diagnostics.Process.Start(document.FilePath);
                }
            }
        }

        private void RenameDocument()
        {
            if (SelectedDocument != null)
            {
                var inputDialog = new Views.InputDialog("Rename Document", "Enter new name:");
                if (inputDialog.ShowDialog() == true)
                {
                    var document = _documentRepository.Get(SelectedDocument.Id);
                    if (document != null)
                    {
                        document.Title = inputDialog.InputText;
                        _documentRepository.Update(document);
                        LoadDocuments();
                    }
                }
            }
        }

        private void DeleteDocument()
        {
            if (SelectedDocument != null)
            {
                _documentRepository.Delete(SelectedDocument.Id);
                LoadDocuments();
            }
        }

        private void LoadDocuments()
        {
            var documents = _documentRepository.GetAll();
            Documents.Clear();
            foreach (var doc in documents)
            {
                var client = _clientRepository.Get(doc.ClientId);
                Documents.Add(new DocumentViewModel(doc)
                {
                    ClientName = client != null ? $"{client.FirstName} {client.LastName}" : "Unknown"
                });
            }
        }

        private void FilterDocuments()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                LoadDocuments();
                return;
            }

            var filteredDocuments = _documentRepository.GetAll().Where(d =>
            {
                var client = _clientRepository.Get(d.ClientId);
                var clientName = client != null ? $"{client.FirstName} {client.LastName}" : "Unknown";
                return clientName.ToLower().Contains(FilterText.ToLower());
            });

            Documents.Clear();
            foreach (var doc in filteredDocuments)
            {
                var client = _clientRepository.Get(doc.ClientId);
                Documents.Add(new DocumentViewModel(doc)
                {
                    ClientName = client != null ? $"{client.FirstName} {client.LastName}" : "Unknown"
                });
            }
        }

        private void AddDocument()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var inputDialog = new Views.InputDialog("Client ID", "Enter Client ID:");
                if (inputDialog.ShowDialog() == true)
                {
                    if (int.TryParse(inputDialog.InputText, out int clientId))
                    {
                        var client = _clientRepository.Get(clientId);
                        if (client != null)
                        {
                            var newDocument = new Document
                            {
                                ClientId = clientId,
                                Title = System.IO.Path.GetFileName(openFileDialog.FileName),
                                FilePath = openFileDialog.FileName,
                                CreatedAt = System.DateTime.Now
                            };
                            _documentRepository.Insert(newDocument);
                            LoadDocuments();
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Client not found.");
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Invalid Client ID.");
                    }
                }
            }
        }
    }
}
