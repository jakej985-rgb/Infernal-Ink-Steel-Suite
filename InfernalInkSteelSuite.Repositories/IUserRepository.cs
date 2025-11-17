using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
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
        bool DeleteUser(string username);
        bool CheckPassword(string username, string plainPassword);
        List<User> GetAllUsers();
        string HashPassword(string plain);
    }
}
