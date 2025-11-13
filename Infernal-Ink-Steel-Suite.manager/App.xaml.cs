using System.Windows;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DatabaseTest.TestConnection(); // Just for now
        }
    }
}
