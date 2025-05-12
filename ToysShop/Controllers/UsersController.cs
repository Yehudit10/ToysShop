using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Services;
using System.Text.Json;
using System.Threading.Tasks;
using Zxcvbn;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToysShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        //GET: api/<UsersController>
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            IEnumerable<User> users =await _userService.GetUsers();
            if (users.Count() > 0)
            return Ok(users);
            return NoContent();
        }




        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetByID(int id)
        {
            User user =await _userService.GetUserById(id);
            if (user == null)
                return NoContent();
           return Ok(user);

        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody]User user)
        {
            int strength = _userService.GetPassStrength(user.Password);
            if (strength < 2)
                return BadRequest("password is not strong enough");
            User newUser =await _userService.AddUser(user);
            return CreatedAtAction(nameof(GetByID), new { id = user.Id }, newUser);
        }


        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<User>> Login(User user)
        {
            User foundUser =await _userService.Login(user);
            if(foundUser!=null)
                return Ok(foundUser);
            return Unauthorized();

        }
        [Route("password")]
        [HttpPost]
        public ActionResult<User> CheckPasswordStrength([FromBody]string password)
        {
            int strength = _userService.GetPassStrength(password);
            return Ok(strength);
        }
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(int id, [FromBody]User user)
        {
            User updatedUser =await _userService.UpdateUser(id, user);
            if (updatedUser==null)
                return NotFound();
            return Ok(updatedUser);

        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
