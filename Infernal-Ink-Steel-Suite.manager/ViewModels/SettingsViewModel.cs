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

        public SettingsViewModel(string connectionString, User currentUser)
        {
            _tabs = new ObservableCollection<SettingsTabViewModel>();
            _selectedTab = null!;
            var shopSettingsRepository = new ShopSettingsRepository(connectionString);
            var userRepository = new UserRepository(connectionString);

            Tabs.Add(new UserProfileTabViewModel(userRepository, currentUser.Username));
            Tabs.Add(new ThemingTabViewModel(shopSettingsRepository));
            Tabs.Add(new LinkedAccountsTabViewModel());

            if (currentUser.Role.Contains("Manager") || currentUser.Role.Contains("Admin"))
            {
                Tabs.Insert(1, new ShopSettingsTabViewModel(shopSettingsRepository));
                Tabs.Insert(2, new ManagerTabViewModel(userRepository));
            }
            if (currentUser.Role.Contains("Admin"))
            {
                Tabs.Insert(3, new AdminTabViewModel(userRepository, shopSettingsRepository));
            }

            SelectedTab = Tabs[0];
        }
    }
}
