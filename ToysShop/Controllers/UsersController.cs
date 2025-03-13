using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToysShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        string filePath = "./userfile.txt";
        //GET: api/<UsersController>
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
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
            return Ok(allUsers);

        }




        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<User> GetByID(int id)
        {
            using (StreamReader reader = System.IO.File.OpenText(filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User user = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user.Id == id)
                        return Ok(user);
                }
            }
            return NoContent();

        }

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult SignUp([FromBody]User user)
        {
            int numberOfUsers = System.IO.File.ReadLines(filePath).Count();
            user.Id = numberOfUsers + 1;
            string userJson = JsonSerializer.Serialize(user);
            System.IO.File.AppendAllText(filePath, userJson + Environment.NewLine);
            return CreatedAtAction(nameof(GetByID), new { id = user.Id }, user);
        }
        [Route("login")]
        [HttpPost]
        public ActionResult<User> Login(User user)
        {
            using (StreamReader reader = System.IO.File.OpenText(filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User currentUser = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user.UserName == currentUser.UserName && user.Password == currentUser.Password)
                        return Ok(currentUser);
                   }
            }
            return NotFound();

        }
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public ActionResult<User> Put(int id, [FromBody]User user)
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
                return NotFound();
                string text = System.IO.File.ReadAllText(filePath);
                text = text.Replace(textToReplace, JsonSerializer.Serialize(user));
                System.IO.File.WriteAllText(filePath, text);
            return Ok(user);


        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
