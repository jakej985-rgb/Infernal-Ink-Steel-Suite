using InfernalInkSteelSuite.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace InfernalInkSteelSuite.Views
{
    public partial class AddEditClientView : Window
    {
        private AddEditClientViewModel _viewModel;

        public AddEditClientView(AddEditClientViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            viewModel.RequestClose += (s, e) => Close();

            Loaded += (s, e) =>
            {
                FirstNameTextBox.Focus();
            };
        }

        private void Avatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectPhotoCommand.CanExecute(null))
            {
                _viewModel.SelectPhotoCommand.Execute(null);
            }
        }
    }
}
