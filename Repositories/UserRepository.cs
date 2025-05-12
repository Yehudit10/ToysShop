using Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ToysShopContext _toysShopContext;
        public UserRepository(ToysShopContext toysShopContext)
        {
            _toysShopContext = toysShopContext;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _toysShopContext.Users.ToListAsync();
        }
        public async Task<User> GetUserById(int id)
        {
            return await _toysShopContext.Users.FindAsync(id);
        }

        public async Task<User> AddUser(User user)
        {
            await _toysShopContext.Users.AddAsync(user);
            await _toysShopContext.SaveChangesAsync();
            return user;
        }
        public async Task<User> Login(User user)
        {
            return await _toysShopContext.Users.Where(u => u.UserName == user.UserName && u.Password == user.Password)?.FirstAsync();
        }
        public async Task<User> UpdateUser(int id, User user)
        {
            _toysShopContext.Users.Update(user);
            _toysShopContext.SaveChangesAsync();
            return user;
        }

    }
}
