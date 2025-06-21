
using DTOs;
using Entities;

namespace Services

{
public interface IUserService
{
    Task<ServiceResult<UserDTO>> AddUser(RegisterUserDTO user);
    int GetPassStrength(string password);
    Task<UserDTO> GetUserById(int id);
    Task<IEnumerable<UserDTO>> GetUsers();
    Task<UserDTO> Login(LoginUserDTO user);
    Task<ServiceResult<UserDTO>> UpdateUser(int id, RegisterUserDTO user);
} }