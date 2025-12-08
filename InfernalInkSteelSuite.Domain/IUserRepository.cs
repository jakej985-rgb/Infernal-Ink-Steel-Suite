using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Domain
{
    public interface IUserRepository
    {
        bool CreateTable();
        User? GetUserById(int userId);
        string? GetUsernameById(int userId);
        bool AddUser(User user);
        bool AddUser(string username, string password, string role);
        bool UpdateUser(User user);
        bool UpdateRole(string username, string role);
        bool UpdatePassword(string username, string password);
        bool UpdateAvatarPath(string username, string avatarPath);
        bool DeleteUser(string username);
        bool CheckPassword(string username, string plainPassword);
        List<User> GetAllUsers();
        List<User> GetActiveUsers();
        string HashPassword(string plain);
        bool UpdateLastLogin(string username);
        bool SetUserActiveStatus(string username, bool isActive);
        bool SoftDeleteUser(string username);
        bool UpdateUserPermissions(string username, string permissionsJson);
        bool UpdateUserDepartment(string username, string department);
        bool UpdateUserCommissionRate(string username, decimal rate);
        bool UpdateUserFontSize(string username, int fontSize);
    }
}
