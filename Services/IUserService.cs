using Entities;

namespace Services
{
    public interface IUserService
    {
        Task<User> AddUser(User user);
        int GetPassStrength(string password);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<User> Login(User user);
        Task<User> UpdateUser(int id, User user);
    }
}