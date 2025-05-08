using Entities;

namespace Services
{
    public interface IUserService
    {
        User AddUser(User user);
        int GetPassStrength(string password);
        User GetUserById(int id);
        IEnumerable<User> GetUsers();
        User Login(User user);
        User UpdateUser(int id, User user);
    }
}