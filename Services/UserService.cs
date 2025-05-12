using Entities;
using Repositories;
using System.Net;
using System.Threading.Tasks;
using Zxcvbn;


namespace Services
{
    public class UserService : IUserService
    {
        
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userRepository.GetUsers();
        }
        public int GetPassStrength(string password)
        {
            return Zxcvbn.Core.EvaluatePassword(password).Score;
        }
        public async Task<User> AddUser(User user)
        {
            if (GetPassStrength(user.Password) < 2)
                throw new ArgumentException("password is too weak");
            return await _userRepository.AddUser(user);
        }
        public async Task<User> Login(User user)
        {
            return await _userRepository.Login(user);
        }
        public async Task<User> UpdateUser(int id, User user)
        {
            return await _userRepository.UpdateUser(id, user);
        }
        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }

    }
}
