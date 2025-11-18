using System;

namespace InfernalInkSteelSuite.Services
{
    public static class SettingsUpdateService
    {
        public static event Action? OnSettingsChanged;

        public static void NotifySettingsChanged()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}
