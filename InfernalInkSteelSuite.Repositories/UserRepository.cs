using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Repositories
{
    public class UserRepository(AppDbContext dbContext) : IUserRepository
    {
        private readonly AppDbContext _db = dbContext;

        public bool CreateTable() => true;

        public string HashPassword(string plain) => ComputeHash(plain);

        private static string ComputeHash(string plain)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public string? GetUsernameById(int userId)
        {
            return _db.Users.Where(u => u.Id == userId).Select(u => u.Username).FirstOrDefault();
        }

        public bool AddUser(User user)
        {
            if (string.IsNullOrEmpty(user.PasswordHash)) user.PasswordHash = HashPassword("password");
            if (string.IsNullOrEmpty(user.Role)) user.Role = "User";
            
            _db.Users.Add(user);
            return _db.SaveChanges() > 0;
        }

        public bool AddUser(string username, string password, string role)
        {
            User u = new()
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = role
            };
            return AddUser(u);
        }

        public bool UpdateUser(User user)
        {
            _db.Users.Update(user);
            user.LastModifiedUtc = DateTime.UtcNow;
            return _db.SaveChanges() > 0;
        }

        public User? GetUserByUsername(string username)
        {
            return _db.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool CheckPassword(string username, string plainPassword)
        {
            var user = GetUserByUsername(username);
            if (user == null) return false;
            return user.PasswordHash == ComputeHash(plainPassword);
        }

        public bool DeleteUser(string username)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                _db.Users.Remove(user);
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateRole(string username, string role)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.Role = role;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdatePassword(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.PasswordHash = HashPassword(password);
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateAvatarPath(string username, string avatarPath)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.AvatarPath = avatarPath;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public List<User> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        public User? GetUserById(int userId)
        {
            return _db.Users.Find(userId);
        }

        public List<User> GetActiveUsers()
        {
            return _db.Users.Where(u => u.IsActive && !((ISyncEntity)u).IsDeleted).ToList();
        }

        public bool UpdateLastLogin(string username)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool SetUserActiveStatus(string username, bool isActive)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.IsActive = isActive;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool SoftDeleteUser(string username)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.IsDeleted = true;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateUserPermissions(string username, string permissionsJson)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.PermissionsJson = permissionsJson;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateUserDepartment(string username, string department)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.Department = department;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateUserCommissionRate(string username, decimal rate)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.CommissionRate = rate;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateUserFontSize(string username, int fontSize)
        {
            var user = GetUserByUsername(username);
            if (user != null)
            {
                user.FontSize = fontSize;
                user.LastModifiedUtc = DateTime.UtcNow;
                return _db.SaveChanges() > 0;
            }
            return false;
        }
    }
}
