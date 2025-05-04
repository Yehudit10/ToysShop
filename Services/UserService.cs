using Entities;
using Repositories;
using System.Net;
using Zxcvbn;


namespace Services
{
    public class UserService : IUserService
    {
        
        private IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public IEnumerable<User> GetUsers()
        {
            return userRepository.GetUsers();
        }
        public int GetPassStrength(string password)
        {
            return Zxcvbn.Core.EvaluatePassword(password).Score;
        }
        public User AddUser(User user)
        {
            if (GetPassStrength(user.Password) < 2)
                throw new ArgumentException("password is too weak");
            return userRepository.AddUser(user);
        }
        public User Login(User user)
        {
            return userRepository.Login(user);
        }
        public User UpdateUser(int id, User user)
        {
            return userRepository.UpdateUser(id, user);
        }
        public User GetUserById(int id)
        {
            return userRepository.GetUserById(id);
        }

    }
}
