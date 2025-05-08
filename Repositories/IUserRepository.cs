using Entities;

namespace Repositories
{
    public interface IUserRepository
    {
        User AddUser(User user);
        User GetUserById(int id);
        IEnumerable<User> GetUsers();
        User Login(User user);
        User UpdateUser(int id, User user);
    }
}