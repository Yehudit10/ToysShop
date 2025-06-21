using Entities;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using DTOs;
using System.Net;
using System.Threading.Tasks;
using Zxcvbn;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class UserService : IUserService
    {
        
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        
        public UserService(IUserRepository userRepository,IMapper mapper,ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            List<UserDTO> returnedUsers = new List<UserDTO>();
            return _mapper.Map<List<UserDTO>>(users);
          
        }
        public int GetPassStrength(string password)
        {
            return Zxcvbn.Core.EvaluatePassword(password).Score;
        }
        private async Task<bool> CheckForExistingUser(User user)
        {
            var users = await _userRepository.GetUsers();
           return users.Any(u => u.UserName.Equals(user.UserName)&&u.Id!=user.Id);
        }
        public async Task<ServiceResult<UserDTO>> AddUser(RegisterUserDTO registerUser)
        {
            
            User user = _mapper.Map<User>(registerUser);
            if (GetPassStrength(user.Password) < 2)
                return ServiceResult<UserDTO>.Error(400, "password is too weak");
            bool existingUser = await CheckForExistingUser(user);
            if (existingUser)
                return ServiceResult<UserDTO>.Error(409, "a user by this name already exists");
            var newUser=await _userRepository.AddUser(user);
            return ServiceResult<UserDTO>.Ok(_mapper.Map<UserDTO>(newUser));
        }
        public async Task<UserDTO> Login(LoginUserDTO user)
        {
            User mappedUser = _mapper.Map<User>(user);
            
            User returnedUser=await _userRepository.Login(mappedUser);
            if (returnedUser!=null)
            {
            _logger.LogInformation($"User {returnedUser.UserName} and password {returnedUser.Password} was logged in succesfully");

            }
            return _mapper.Map<UserDTO>(returnedUser);
        }
        public async Task<ServiceResult<UserDTO>> UpdateUser(int id, RegisterUserDTO userToUpdate)
        {
            User user = _mapper.Map<User>(userToUpdate);
            user.Id = id;
            if (GetPassStrength(user.Password) < 2)
                return ServiceResult<UserDTO>.Error(400, "password is too weak");
            bool existingUser = await CheckForExistingUser(user);
            if (existingUser)
                return ServiceResult<UserDTO>.Error(409, "a user by this name already exists");
            var foundUser = await _userRepository.GetUserById(user.Id);
            if (foundUser == null)
                return ServiceResult<UserDTO>.Error(404, "no such user");

            var updatedUser = await _userRepository.UpdateUser(id, user);
           
            return ServiceResult<UserDTO>.Ok(_mapper.Map<UserDTO>(updatedUser));
        }
        public async Task<UserDTO> GetUserById(int id)
        {
            User user = await _userRepository.GetUserById(id);
            return _mapper.Map< UserDTO>(user);
            
        }

    }
}
