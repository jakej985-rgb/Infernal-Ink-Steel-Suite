using System;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.DataProviders
{
    public static class DataProviderFactory
    {
        public static IDataProvider Create(ConnectionSettings settings)
        {
            switch (settings.Mode)
            {
                case DataMode.LocalOnly:
                    return new LocalDataProvider(settings.LocalDbPath, settings.LocalUploadRoot);

                case DataMode.ServerWithOfflineCache:
                    return CreateSyncingProvider(settings);

                default:
                    return new LocalDataProvider(settings.LocalDbPath, settings.LocalUploadRoot);
            }
        }

        private static IDataProvider CreateSyncingProvider(ConnectionSettings settings)
        {
            var localProvider = new LocalDataProvider(settings.LocalDbPath, settings.LocalUploadRoot);
            var syncQueue = new SyncQueueRepository($"Data Source={settings.LocalDbPath}");
            return new SyncingDataProvider(localProvider, syncQueue);
        }
    }
}
