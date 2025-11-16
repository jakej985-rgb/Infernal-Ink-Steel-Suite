using System.Windows;
using System.Windows.Controls;

namespace Infernal_Ink_Steel_Suite.manager.Views
{
    public class InputDialog : Window
    {
        private TextBox inputTextBox;
        private Button okButton;

        public string InputText => inputTextBox.Text;

        public InputDialog(string title, string prompt)
        {
            Title = title;

            var promptLabel = new Label { Content = prompt };
            inputTextBox = new TextBox();
            okButton = new Button { Content = "OK" };
            okButton.Click += (sender, e) => DialogResult = true;

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(promptLabel);
            stackPanel.Children.Add(inputTextBox);
            stackPanel.Children.Add(okButton);

            Content = stackPanel;
        }
    }
}
