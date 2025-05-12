using Entities;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<User> Login(User user);
        Task<User> UpdateUser(int id, User user);
    }
}