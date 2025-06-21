
using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class UserRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly ToysShopContext _toysShopContext;
        private readonly UserRepository _userRepository;
        public UserRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _toysShopContext = databaseFixture.Context;
            _userRepository = new UserRepository(_toysShopContext);
        }
        [Fact]
        public async Task GetAllUsers_ReturnUsersAsync()
        {
            // Arrange
            _toysShopContext.Users.RemoveRange(_toysShopContext.Users);
            await _toysShopContext.SaveChangesAsync();

            var user1 = new User { UserName = "user1", Password = "pass1" };
            var user2 = new User { UserName = "user2", Password = "pass2" };

            await _userRepository.AddUser(user1);
            await _userRepository.AddUser(user2);

            // Act
            var users = (await _userRepository.GetUsers()).ToList();

            // Assert
            Assert.Equal(2, users.Count());

            Assert.Contains(users, u => u.UserName == "user1" && u.Password == "pass1");
            Assert.Contains(users, u => u.UserName == "user2" && u.Password == "pass2");
        }
        [Fact]
        public async Task GetUsers_WhenEmpty_ShouldReturnEmptyList()
        {
            _toysShopContext.Users.RemoveRange(_toysShopContext.Users);
            await _toysShopContext.SaveChangesAsync();
            var users = await _userRepository.GetUsers();

            Assert.Empty(users);
        }
        [Fact]
        public async Task AddUser_ShouldAddUser()
        {
            var user = new User { UserName = "testuser", Password = "pass123" };

            var result = await _userRepository.AddUser(user);

           
            var savedUser = await _toysShopContext.Users.FindAsync(result.Id);
            Assert.NotNull(savedUser);
            Assert.Equal("testuser", savedUser.UserName);

        }
        [Fact]
        public async Task GetUserById_ShouldReturnCorrectUser()
        {
            var user = new User { UserName = "specific", Password = "123" };
            _toysShopContext.Users.Add(user);
            await _toysShopContext.SaveChangesAsync();

            var fetched = await _userRepository.GetUserById(user.Id);
            Assert.NotNull(fetched);
            Assert.Equal("specific", fetched.UserName);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNullForNonexistentId()
        {
            
            var result = await _userRepository.GetUserById(-1);
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_ShouldReturnUserIfCredentialsMatch()
        {
            var user = await _toysShopContext.Users.AddAsync(new User { UserName = "loginuser", Password = "secret" });
            await _toysShopContext.SaveChangesAsync();
            var loggedIn = await _userRepository.Login(new User { UserName = "loginuser", Password = "secret" });

            Assert.NotNull(loggedIn);
            Assert.Equal(loggedIn.UserName, "loginuser");
        }
        [Fact]
        public async Task Login_WithIncorrectPassword_ShouldReturnNull()
        {
            var user = new User { UserName = "secure", Password = "realpass" };
            await _toysShopContext.Users.AddAsync(user);
            await _toysShopContext.SaveChangesAsync();

            var result = await _userRepository.Login(new User { UserName = "secure", Password = "wrongpass" });

            Assert.Null(result);
        }
        [Fact]
        public async Task Login_WithNonExistentUser_ShouldReturnNull()
        {
            

            var loggedIn = await _userRepository.Login(new User { UserName = "wronguser", Password = "wrong" });

            Assert.Null(loggedIn);
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyExistingUser()
        {
            // Arrange
            var user = new User { UserName = "oldname", Password = "oldpass" };
            await _toysShopContext.Users.AddAsync(user);
            await _toysShopContext.SaveChangesAsync();

            // Act
            user.UserName = "newname";
            user.Password = "newpass";
            var updated = await _userRepository.UpdateUser(user.Id, user);

            
            Assert.Equal("newname", updated.UserName);
            Assert.Equal("newpass", updated.Password);
            var check = await _toysShopContext.Users.FindAsync(user.Id);
            Assert.Equal("newname", check.UserName);
            Assert.Equal("newpass", check.Password);
        }

      


    }
}
