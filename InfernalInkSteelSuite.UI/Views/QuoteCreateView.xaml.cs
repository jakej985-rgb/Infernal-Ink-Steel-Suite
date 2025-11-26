using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InfernalInkSteelSuite.UI.ViewModels;

namespace InfernalInkSteelSuite.UI.Views
{
    public partial class QuoteCreateView : UserControl
    {
        public QuoteCreateView()
        {
            InitializeComponent();
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            // No visual change on drag leave in this implementation
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is QuoteCreateViewModel viewModel && viewModel.DropImageCommand.CanExecute(e.Data))
            {
                viewModel.DropImageCommand.Execute(e.Data);
            }
        }
    }
}
