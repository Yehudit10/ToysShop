using Entities;
using Repositories;
using Moq;
using Moq.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace Tests
{
    public class UserRepositoryUnitTests
    {
        private List<User> GetTestUsers() => new List<User>
    {
        new User { Id = 1, UserName = "user1", Password = "pass1",FirstName="Yehudit",LastName="Bushrian" },
        new User { Id = 2, UserName = "user2", Password = "pass2" },
    };

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var repo = new UserRepository(mockContext.Object);

            var result = await repo.GetUsers();
           // Assert.Equal(2, result.Count());
            Assert.Equal(result, users);//??
        }
        [Fact]
        public async Task GetUsers_ReturnsEmptyList()
        {
            
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(new List<User>());
            var repo = new UserRepository(mockContext.Object);
            var result = await repo.GetUsers();
            Assert.Equal( result.Count(),0);
            Assert.Empty(result);
            
        }

        [Fact]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);
            mockContext.Setup(c => c.Users.FindAsync(1))
                .ReturnsAsync(users[0]);
          

     //       mockContext.Setup(c => c.Users.FindAsync(It.IsAny<object[]>()))
     //.ReturnsAsync((object[] ids) =>
     //{
     //    var id = (int)ids[0];
     //    return users.FirstOrDefault(u => u.Id == id);
     //});

            var repo = new UserRepository(mockContext.Object);

            var result = await repo.GetUserById(1);

            Assert.Equal(result, users[0]);


        }

        [Fact]
        public async Task GetUserById_InvalidId_ReturnsNull()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);
            mockContext.Setup(c => c.Users.FindAsync(99))
               .ReturnsAsync((User)null);
   
            var repo = new UserRepository(mockContext.Object);

            var result = await repo.GetUserById(99);

            Assert.Null(result);
        }
        [Fact]
        public async Task AddUser_AddsUserAndSaves()
        {

            var mockContext = new Mock<ToysShopContext>();
            var newUser = new User { Id = 3, UserName = "new", Password = "newpass" };
            List<User> users = new List<User>();
            var mockDbSet = new Mock<DbSet<User>>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(new List<User>());
       
            mockContext.Setup(c => c.Users.AddAsync(newUser, default))
    .Returns((User u,CancellationToken ct) => { users.Add(u); 
        return new ValueTask<EntityEntry<User>>((EntityEntry<User>?)null); });
            mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
            var repo = new UserRepository(mockContext.Object);

            var result = await repo.AddUser(newUser);
            mockContext.Verify(c => c.Users.AddAsync(newUser, default), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.Equal(result,newUser);
            Assert.Contains(result, users);
        }
       
        [Fact]
        public async Task Login_ValidCredentials_ReturnsUser()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var repo = new UserRepository(mockContext.Object);

            var loginUser = new User { UserName = "user1", Password = "pass1" };
            var result = await repo.Login(loginUser);
            Assert.Equal(result, users[0]);

        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsNull()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var repo = new UserRepository(mockContext.Object);

            var loginUser = new User { UserName = "wrong", Password = "wrong" };
            var result = await repo.Login(loginUser);

            Assert.Null(result);
        }
        [Fact]
        public async Task Login_MissingFields_ReturnsNull()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);

            var repo = new UserRepository(mockContext.Object);

            var result = await repo.Login(new User { UserName = "aa" });

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUser_UpdatesUserAndSaves()
        {
            var users = GetTestUsers();
            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(c => c.Users).ReturnsDbSet(users);
            var updatedUser = new User { Id = 1, UserName = "updated", Password = "updatedpass", LastName = "Bushrain2" };

            var repo = new UserRepository(mockContext.Object);

            var result = await repo.UpdateUser(1, updatedUser);

            Assert.Equal(updatedUser, result);
        }
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsUser()
        {
            User user = new User { UserName = "yehudit", Password = "test" };
            List<User> users = new List<User> { user };
            var moqContext = new Mock<ToysShopContext>();
            moqContext.Setup(x => x.Users).ReturnsDbSet(users);
            UserRepository userRepository = new UserRepository(moqContext.Object);
            var result = await userRepository.Login(user);
            Assert.Equal(result, user);

        }
        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsNull()
        {
            User existingUser = new User { UserName = "yehudit", Password = "test" };
            List<User> users = new List<User> { existingUser };

            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);
            UserRepository userRepository = new UserRepository(mockContext.Object);
            var loginAttempt = new User { UserName = "yehudit", Password = "invalid" };
            var result = await userRepository.Login(loginAttempt);
            Assert.Null(result);
        }
        [Fact]

        public async Task SignUp_WithValidPassword_ReturnsUser()
        {
            User existingUser = new User { UserName = "yehudit", Password = "test" };
            List<User> users = new List<User> { existingUser };

            var mockContext = new Mock<ToysShopContext>();
            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            UserRepository userRepository = new UserRepository(mockContext.Object);
            var signUpAttempt = new User { UserName = "yehudit", Password = "invalid" };
            var result = await userRepository.AddUser(signUpAttempt);

            Assert.Equal(signUpAttempt, result);
        }
    }
}
