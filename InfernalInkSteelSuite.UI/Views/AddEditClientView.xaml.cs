using InfernalInkSteelSuite.ViewModels;
using System.Windows;

namespace InfernalInkSteelSuite.UI.Views
{
    public partial class AddEditClientView : Window
    {
        public AddEditClientView(AddEditClientViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += (s, e) => Close();

            Loaded += (s, e) =>
            {
                FirstNameTextBox.Focus();
            };
        }
    }
}
