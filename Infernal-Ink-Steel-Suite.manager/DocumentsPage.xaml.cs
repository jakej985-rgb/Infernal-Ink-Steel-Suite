using InfernalInkSteelSuite.Repositories;
using System.Windows;
using System.Windows.Controls;

namespace Infernal_Ink_Steel_Suite.manager
{
    public partial class DocumentsPage : Page
    {
        private readonly DocumentRepository _documentRepository;

        public DocumentsPage()
        {
            InitializeComponent();
            _documentRepository = new DocumentRepository();
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            DocumentsGrid.ItemsSource = _documentRepository.GetAllDocuments();
        }

        private void AddDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            // Add document logic here
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDocuments();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Filter logic here
        }
    }
}
