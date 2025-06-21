using Xunit;
using Moq;
using Services;
using Repositories;
using Entities;
using DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Services.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        // Test data
        private readonly User _testUser;
        private readonly UserDTO _testUserDTO;
        private readonly RegisterUserDTO _testRegisterUserDTO;
        private readonly LoginUserDTO _testLoginUserDTO;
        private readonly List<User> _testUsers;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object);

            // Initialize test data
            _testUser = new User
            {
                Id = 1,
                UserName = "testuser",
                Password = "StrongPassword123!"
            };

            _testUserDTO = new UserDTO(1,"testuser",null,null);
            //{
            //    Id = 1,
            //    UserName = "testuser"
            //};

            _testRegisterUserDTO = new RegisterUserDTO("StrongPassword123!", "testuser",null,null);
            //{
            //    Password = "StrongPassword123!",
            //    UserName = "testuser",

            //};

            _testLoginUserDTO = new LoginUserDTO("testuser", "StrongPassword123!");
            //{
            //    UserName = "testuser",
            //    Password = "StrongPassword123!",
                
            //};

            _testUsers = new List<User>
            {
                _testUser,
                new User { Id = 2, UserName = "user2", Password = "AnotherPassword123!" }
            };
        }

        #region GetUsers Tests

        [Fact]
        public async Task GetUsers_WhenCalled_ShouldReturnMappedUserDTOs()
        {
            // Arrange
            var expectedUserDTOs = new List<UserDTO> { _testUserDTO };
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(_testUsers);
            _mockMapper.Setup(m => m.Map<List<UserDTO>>(_testUsers)).Returns(expectedUserDTOs);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedUserDTOs.First().Id, result.First().Id);
            Assert.Equal(expectedUserDTOs.First().UserName, result.First().UserName);
            _mockUserRepository.Verify(r => r.GetUsers(), Times.Once);
            _mockMapper.Verify(m => m.Map<List<UserDTO>>(_testUsers), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WhenNoUsers_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyUsers = new List<User>();
            var emptyUserDTOs = new List<UserDTO>();
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(emptyUsers);
            _mockMapper.Setup(m => m.Map<List<UserDTO>>(emptyUsers)).Returns(emptyUserDTOs);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        

        #endregion

        #region GetPassStrength Tests

       

        [Fact]
        public void GetPassStrength_WithWeakPassword_ShouldReturnScoreLessThan2()
        {
            // Act
            var result = _userService.GetPassStrength("123");

            // Assert
            Assert.True(result < 2, $"Weak password should have score < 2, got {result}");
        }

        [Fact]
        public void GetPassStrength_WithStrongPassword_ShouldReturnScoreGreaterOrEqual2()
        {
            // Act
            var result = _userService.GetPassStrength("StrongPassword123!");

            // Assert
            Assert.True(result >= 2, $"Strong password should have score >= 2, got {result}");
        }

      

     

        #endregion

        #region AddUser Tests

        [Fact]
        public async Task AddUser_WithValidUser_ShouldReturnSuccessResult()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns(_testUser);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(new List<User>());
            _mockUserRepository.Setup(r => r.AddUser(_testUser)).ReturnsAsync(_testUser);
            _mockMapper.Setup(m => m.Map<UserDTO>(_testUser)).Returns(_testUserDTO);

            // Act
            var result = await _userService.AddUser(_testRegisterUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(_testUserDTO.Id, result.Data.Id);
            Assert.Equal(_testUserDTO.UserName, result.Data.UserName);
            _mockUserRepository.Verify(r => r.AddUser(_testUser), Times.Once);
        }

        [Fact]
        public async Task AddUser_WithWeakPassword_ShouldReturnError()
        {
            // Arrange
            var weakPasswordUser = new RegisterUserDTO("123", "testuser", null, null);
            //{
            //    UserName = "testuser",
            //    Password = "123" // Very weak password
            //};
            var mappedUser = new User { UserName = "testuser", Password = "123" };
            _mockMapper.Setup(m => m.Map<User>(weakPasswordUser)).Returns(mappedUser);

            // Act
            var result = await _userService.AddUser(weakPasswordUser);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.ErrorStatus);
            Assert.Equal("password is too weak", result.ErrorMessage);
            _mockUserRepository.Verify(r => r.AddUser(It.IsAny<User>()), Times.Never);
        }
    
        [Fact]
        public async Task AddUser_WithExistingUsername_ShouldReturnError()
        {
            // Arrange
        User dupUser=new User { Id=5,UserName="testuser",Password= "t56g6bd" };
       RegisterUserDTO dupRegisterUserDTO=new RegisterUserDTO("t56g6bd","testuser",null,null);
        _mockMapper.Setup(m => m.Map<User>(dupRegisterUserDTO)).Returns(dupUser);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(_testUsers);

            // Act
            var result = await _userService.AddUser(dupRegisterUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.ErrorStatus);
            Assert.Equal("a user by this name already exists", result.ErrorMessage);
            _mockUserRepository.Verify(r => r.AddUser(It.IsAny<User>()), Times.Never);
        }

        

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnUserDTO()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<User>(_testLoginUserDTO)).Returns(_testUser);
            _mockUserRepository.Setup(r => r.Login(_testUser)).ReturnsAsync(_testUser);
            _mockMapper.Setup(m => m.Map<UserDTO>(_testUser)).Returns(_testUserDTO);

            // Act
            var result = await _userService.Login(_testLoginUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testUserDTO.Id, result.Id);
            Assert.Equal(_testUserDTO.UserName, result.UserName);
            _mockUserRepository.Verify(r => r.Login(_testUser), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldLogSuccessfulLogin()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<User>(_testLoginUserDTO)).Returns(_testUser);
            _mockUserRepository.Setup(r => r.Login(_testUser)).ReturnsAsync(_testUser);
            _mockMapper.Setup(m => m.Map<UserDTO>(_testUser)).Returns(_testUserDTO);

            // Act
            await _userService.Login(_testLoginUserDTO);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("was logged in succesfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Login_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<User>(_testLoginUserDTO)).Returns(_testUser);
            _mockUserRepository.Setup(r => r.Login(_testUser)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.Login(_testLoginUserDTO));
        }

        [Fact]
        public async Task Login_WhenUserNotFound_ShouldPropagateRepositoryBehavior()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<User>(_testLoginUserDTO)).Returns(_testUser);
            _mockUserRepository.Setup(r => r.Login(_testUser)).ReturnsAsync((User)null);
            _mockMapper.Setup(m => m.Map<UserDTO>((User)null)).Returns((UserDTO)null);

            // Act
            var result = await _userService.Login(_testLoginUserDTO);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region UpdateUser Tests

        [Fact]
        public async Task UpdateUser_WithValidUser_ShouldReturnSuccessResult()
        {
            // Arrange
            var userToUpdate = new User { Id = 1, UserName = "updateduser", Password = "StrongPassword123!" };
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns(userToUpdate);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(new List<User>());
            _mockUserRepository.Setup(r => r.GetUserById(1)).ReturnsAsync(_testUser);
            _mockUserRepository.Setup(r => r.UpdateUser(1, userToUpdate)).ReturnsAsync(userToUpdate);
            _mockMapper.Setup(m => m.Map<UserDTO>(userToUpdate)).Returns(_testUserDTO);

            // Act
            var result = await _userService.UpdateUser(1, _testRegisterUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(_testUserDTO.Id, result.Data.Id);
            Assert.Equal(_testUserDTO.UserName, result.Data.UserName);
            _mockUserRepository.Verify(r => r.UpdateUser(1, userToUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithWeakPassword_ShouldReturnError()
        {
            // Arrange
            var weakPasswordUser = new RegisterUserDTO("123", "testuser", null, null);
            //{
            //    UserName = "testuser",
            //    Password = "123"
            //};
            var mappedUser = new User { Id = 1, UserName = "testuser", Password = "123" };
            _mockMapper.Setup(m => m.Map<User>(weakPasswordUser)).Returns(mappedUser);

            // Act
            var result = await _userService.UpdateUser(1, weakPasswordUser);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.ErrorStatus);
            Assert.Equal("password is too weak", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateUser_WithExistingUsername_ShouldReturnError()
        {
            // Arrange
            var userToUpdate = new User { Id = 1, UserName = "user2", Password = "StrongPassword123!" };
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns(userToUpdate);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(_testUsers);

            // Act
            var result = await _userService.UpdateUser(1, _testRegisterUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.ErrorStatus);
            Assert.Equal("a user by this name already exists", result.ErrorMessage);
        }


        [Fact]
        public async Task UpdateUser_WithNonExistentUser_ShouldReturnError()
        {
            // Arrange
            var userToUpdate = new User { Id = 999, UserName = "newuser", Password = "StrongPassword123!" };
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns(userToUpdate);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(new List<User>());
            _mockUserRepository.Setup(r => r.GetUserById(999)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateUser(999, _testRegisterUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.ErrorStatus);
            Assert.Equal("no such user", result.ErrorMessage);
        }

       

        [Fact]
        public async Task UpdateUser_WhenRepositoryThrows_ShouldPropagateException()
        {
            // Arrange
            var userToUpdate = new User { Id = 1, UserName = "updateduser", Password = "StrongPassword123!" };
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns(userToUpdate);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(new List<User>());
            _mockUserRepository.Setup(r => r.GetUserById(1)).ReturnsAsync(_testUser);
            _mockUserRepository.Setup(r => r.UpdateUser(1, userToUpdate)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUser(1, _testRegisterUserDTO));
        }

        #endregion

        #region GetUserById Tests

        [Fact]
        public async Task GetUserById_WithValidId_ShouldReturnUserDTO()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetUserById(1)).ReturnsAsync(_testUser);
            _mockMapper.Setup(m => m.Map<UserDTO>(_testUser)).Returns(_testUserDTO);

            // Act
            var result = await _userService.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testUserDTO.Id, result.Id);
            Assert.Equal(_testUserDTO.UserName, result.UserName);
            _mockUserRepository.Verify(r => r.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetUserById_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetUserById(999)).ReturnsAsync((User)null);
            _mockMapper.Setup(m => m.Map<UserDTO>((User)null)).Returns((UserDTO)null);

            // Act
            var result = await _userService.GetUserById(999);

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(r => r.GetUserById(999), Times.Once);
        }


        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public async Task AddUser_WhenMapperReturnsNull_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _userService.AddUser(_testRegisterUserDTO));
        }

        [Fact]
        public async Task UpdateUser_WhenRepositoryReturnsNullForUpdate_ShouldReturnSuccessWithNullData()
        {
            // Arrange
            var userToUpdate = new User { Id = 1, UserName = "updateduser", Password = "StrongPassword123!" };
            _mockMapper.Setup(m => m.Map<User>(_testRegisterUserDTO)).Returns(userToUpdate);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(new List<User>());
            _mockUserRepository.Setup(r => r.GetUserById(1)).ReturnsAsync(_testUser);
            _mockUserRepository.Setup(r => r.UpdateUser(1, userToUpdate)).ReturnsAsync((User)null);
            _mockMapper.Setup(m => m.Map<UserDTO>((User)null)).Returns((UserDTO)null);

            // Act
            var result = await _userService.UpdateUser(1, _testRegisterUserDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Null(result.Data);
        }

    

        [Fact]
        public async Task CheckForExistingUser_Logic_ShouldWorkCorrectlyThroughPublicMethods()
        {
            // This tests the private CheckForExistingUser method indirectly
            // Arrange
            var newUser = new RegisterUserDTO("StrongPassword123", "newuser",null,null);
            //{ UserName = "newuser", Password = "StrongPassword123!" };
            var mappedNewUser = new User { Id = 0, UserName = "newuser", Password = "StrongPassword123!" };

            _mockMapper.Setup(m => m.Map<User>(newUser)).Returns(mappedNewUser);
            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(_testUsers);

            // Act
            var result = await _userService.AddUser(newUser);

            // Assert - Should succeed because username doesn't exist
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        #endregion
    }
}
