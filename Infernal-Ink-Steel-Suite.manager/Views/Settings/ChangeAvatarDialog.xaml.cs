using InfernalInkSteelSuite.ViewModels.Settings;
using System.Windows;

namespace InfernalInkSteelSuite.Views.Settings
{
    public partial class ChangeAvatarDialog : Window
    {
        public ChangeAvatarDialog()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                if (DataContext is ChangeAvatarDialogViewModel vm)
                {
                    vm.CloseAction = Close;
                }
            };
        }
    }
}
