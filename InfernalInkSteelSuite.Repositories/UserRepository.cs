namespace InfernalInkSteelSuite.Repositories
{
    public class UserRepository
    {
        public IEnumerable<InfernalInkSteelSuite.Domain.User> GetAllUsers()
        {
            // Implementation here
            return new List<InfernalInkSteelSuite.Domain.User>();
        }

        public InfernalInkSteelSuite.Domain.User GetUserByUsername(string username)
        {
            // Implementation here
            return null;
        }
    }
}