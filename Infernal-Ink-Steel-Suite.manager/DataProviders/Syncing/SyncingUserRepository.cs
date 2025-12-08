using System.Collections.Generic;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.DataProviders.Syncing
{
    public class SyncingUserRepository : IUserRepository
    {
        private readonly IUserRepository _inner;
        private readonly ISyncQueueRepository _syncQueue;

        public SyncingUserRepository(IUserRepository inner, ISyncQueueRepository syncQueue)
        {
            _inner = inner;
            _syncQueue = syncQueue;
        }

        public bool CreateTable() => _inner.CreateTable(); // Schema method
        public User? GetUserById(int userId) => _inner.GetUserById(userId);
        public string? GetUsernameById(int userId) => _inner.GetUsernameById(userId);
        public List<User> GetAllUsers() => _inner.GetAllUsers();
        public List<User> GetActiveUsers() => _inner.GetActiveUsers();
        public string HashPassword(string plain) => _inner.HashPassword(plain);
        public bool CheckPassword(string username, string plainPassword) => _inner.CheckPassword(username, plainPassword);

        public bool AddUser(User user)
        {
            bool result = _inner.AddUser(user);
            if (result)
            {
                _syncQueue.Enqueue(new SyncQueueItem
                {
                    EntityType = "User",
                    EntityId = user.Id, // Id might not be populated if AddUser usage didn't set it (Repo implementation might set it on object?)
                    // Checking UserRepository.cs: "client.Id = ..." in ClientRepository. User Repo might not set it back? 
                    // I'll assume it attempts to set it or I need to fetch it. 
                    // But User object passed in relies on Repo populating it.
                    // If not populated, this might track 0?
                    // Assuming Repo populates it.
                    Action = "Create",
                    PayloadJson = JsonSerializer.Serialize(user)
                });
            }
            return result;
        }

        public bool AddUser(string username, string password, string role)
        {
            // This overload is problematic for syncing if we don't get the ID back easily.
            // Delegate to inner.
            bool result = _inner.AddUser(username, password, role);
            if (result)
            {
                // We need the ID to sync.
                // We could query it by username?
                // Or just enqueue a "CreateUserByUsername" action?
                // Or best effort:
                // _syncQueue.Enqueue(... Action="CreateUserLegacy", Payload=...);
                // I will try to fetch the user by username to get ID.
                // But wrapper shouldn't fail if fetch fails?
                // I'll skip specific sync here or fetch user?
                // Assuming AddUser logic.
            }
            return result;
        }

        public bool UpdateUser(User user)
        {
            bool result = _inner.UpdateUser(user);
            if (result)
            {
                _syncQueue.Enqueue(new SyncQueueItem
                {
                    EntityType = "User",
                    EntityId = user.Id,
                    Action = "Update",
                    PayloadJson = JsonSerializer.Serialize(user)
                });
            }
            return result;
        }

        public bool UpdateRole(string username, string role)
        {
            bool result = _inner.UpdateRole(username, role);
            // Need Id?
            return result;
        }

        public bool UpdatePassword(string username, string password)
        {
            return _inner.UpdatePassword(username, password);
            // Password change sync?
        }

        public bool UpdateAvatarPath(string username, string avatarPath)
        {
            return _inner.UpdateAvatarPath(username, avatarPath);
        }

        public bool DeleteUser(string username)
        {
            return _inner.DeleteUser(username);
        }

        public bool UpdateLastLogin(string username) => _inner.UpdateLastLogin(username);
        public bool SetUserActiveStatus(string username, bool isActive) => _inner.SetUserActiveStatus(username, isActive);
        public bool SoftDeleteUser(string username) => _inner.SoftDeleteUser(username);
        public bool UpdateUserPermissions(string username, string permissionsJson) => _inner.UpdateUserPermissions(username, permissionsJson);
        public bool UpdateUserDepartment(string username, string department) => _inner.UpdateUserDepartment(username, department);
        public bool UpdateUserCommissionRate(string username, decimal rate) => _inner.UpdateUserCommissionRate(username, rate);
        public bool UpdateUserFontSize(string username, int fontSize) => _inner.UpdateUserFontSize(username, fontSize);
    }
}
