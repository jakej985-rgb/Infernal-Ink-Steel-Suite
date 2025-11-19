using InfernalInkSteelSuite.ViewModels.Settings;
using System.Windows;

namespace InfernalInkSteelSuite.Views.Settings
{
    public partial class ChangePasswordDialog : Window
    {
        public ChangePasswordDialog()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                if (DataContext is ChangePasswordDialogViewModel vm)
                {
                    vm.CloseAction = Close;
                }
            };
        }
    }
}
