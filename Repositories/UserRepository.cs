using Entities;
using System.Text.Json;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        string filePath = "./userfile.txt";

        public IEnumerable<User> GetUsers()
        {
            List<User> allUsers = new List<User>();
            using (StreamReader reader = System.IO.File.OpenText(filePath))
            {
                string? currentUser;
                while ((currentUser = reader.ReadLine()) != null)
                {
                    User serilizedUser = JsonSerializer.Deserialize<User>(currentUser);
                    allUsers.Add(serilizedUser);
                }
            }
            return allUsers;
        }
        public User GetUserById(int id)
        {
            using (StreamReader reader = System.IO.File.OpenText(filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User user = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user.Id == id)
                        return user;
                }
            }
            return null;
        }

        public User AddUser(User user)
        {
            int numberOfUsers = System.IO.File.ReadLines(filePath).Count();
            user.Id = numberOfUsers + 1;
            string userJson = JsonSerializer.Serialize(user);
            System.IO.File.AppendAllText(filePath, userJson + Environment.NewLine);
            return user;
        }
        public User Login(User user)
        {
            using (StreamReader reader = System.IO.File.OpenText(filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User currentUser = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user.UserName == currentUser.UserName && user.Password == currentUser.Password)
                        return currentUser;
                }
            }
            return null;
        }
        public User UpdateUser(int id, User user)
        {
            string textToReplace = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText(filePath))
            {
                string currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {

                    User currentUser = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (currentUser.Id == id)
                        textToReplace = currentUserInFile;
                }
            }
            if (textToReplace == string.Empty)
                return null;
            string text = System.IO.File.ReadAllText(filePath);
            text = text.Replace(textToReplace, JsonSerializer.Serialize(user));
            System.IO.File.WriteAllText(filePath, text);
            return user;
        }

    }
}
