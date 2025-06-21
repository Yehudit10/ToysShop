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
            var users = await _toysShopContext.Users.AsNoTracking().ToListAsync();
            return users;
        }
        public async Task<User> GetUserById(int id)
        {
           //_toysShopContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var user= await _toysShopContext.Users.FindAsync(id);
            if (user != null&&_toysShopContext.Entry(user)!=null)
                _toysShopContext.Entry(user).State = EntityState.Detached;
            return user;
        }

        public async Task<User> AddUser(User user)
        {
           
            await _toysShopContext.Users.AddAsync(user);
            await _toysShopContext.SaveChangesAsync();
            return user;
        }
        public async Task<User> Login(User user)
        {
            
            User foundUser= await _toysShopContext.Users.Where(u => u.UserName == user.UserName && u.Password == user.Password)?.FirstOrDefaultAsync();
            return foundUser;
        }
        public async Task<User> UpdateUser(int id, User user)
        {

           
           

            //_toysShopContext.Users.Update(new User { Id = 8, UserName = "dfef", Password = "fv4jrj38ug" });
            _toysShopContext.Users.Update(user);
            await _toysShopContext.SaveChangesAsync();
            return user;
        }

    }
}
