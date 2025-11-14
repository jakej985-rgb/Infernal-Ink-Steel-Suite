using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IUserRepository
    {
        User GetUserById(int userId);
        string GetUsernameById(int userId);
        bool AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int userId);
    }
}
