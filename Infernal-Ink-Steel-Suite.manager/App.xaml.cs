using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using InfernalInkSteelSuite.Data;

namespace Infernal_Ink_Steel_Suite.manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DatabaseManager.InitializeDatabase();

            var loginWindow = new Window
            {
                Title = "Login",
                Content = new LoginPage(),
                Width = 300,
                Height = 200
            };
            loginWindow.Show();
        }
    }
}
