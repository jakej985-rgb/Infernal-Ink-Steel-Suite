using InfernalInkSteelSuite.ViewModels.Settings;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private ObservableCollection<SettingsTabViewModel> _tabs;
        public ObservableCollection<SettingsTabViewModel> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                OnPropertyChanged();
            }
        }

        private SettingsTabViewModel _selectedTab;
        public SettingsTabViewModel SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel(IDataProvider dataProvider, User currentUser)
        {
            _tabs = [];
            _selectedTab = null!;

            Tabs.Add(new UserTabViewModel(dataProvider.Users, dataProvider.ShopSettings, currentUser));
            Tabs.Add(new LinkedAccountsTabViewModel(dataProvider.ShopSettings));
            Tabs.Add(new NotificationSettingsTabViewModel(dataProvider.ShopSettings));
            Tabs.Add(new BackupDataTabViewModel(dataProvider.ShopSettings));
            Tabs.Add(new AccessibilityTabViewModel(dataProvider.Users, currentUser));

            if (currentUser.Role.Contains("Manager") || currentUser.Role.Contains("Admin"))
            {
                Tabs.Insert(2, new ManagerTabViewModel(dataProvider.Users));
            }
            if (currentUser.Role.Contains("Admin"))
            {
                Tabs.Insert(1, new AdminTabViewModel(dataProvider.Users, dataProvider.ShopSettings));
            }

            SelectedTab = Tabs[0];
        }
    }
}
