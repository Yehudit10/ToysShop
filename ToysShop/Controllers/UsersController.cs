using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Services;
using System.Net;
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
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            IEnumerable<UserDTO> users =await _userService.GetUsers();
            if (users.Count() > 0)
            return Ok(users);
            return NoContent();
        }




        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetByID(int id)
        {
            UserDTO user =await _userService.GetUserById(id);
            if (user == null)
                return NoContent();
           return Ok(user);

        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody]RegisterUserDTO user)
        {
            //int strength = _userService.GetPassStrength(user.Password);
            //if (strength < 2)
            //    return BadRequest("password is not strong enough");
            ServiceResult<UserDTO > result= await _userService.AddUser(user); 
            if (!result.IsSuccess)
                return StatusCode((int)result.ErrorStatus, result.ErrorMessage); 
            return CreatedAtAction(nameof(GetByID), new { id = result.Data.Id }, result.Data);
        }


        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login(LoginUserDTO user)
        {
            UserDTO foundUser =await _userService.Login(user);
            if(foundUser!=null)
                return Ok(foundUser);
            return Unauthorized();

        }
        [Route("password")]
        [HttpPost]
        public ActionResult<int> CheckPasswordStrength([FromBody]string password)
        {
            int strength = _userService.GetPassStrength(password);
            return Ok(strength);
        }
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Put(int id, [FromBody] RegisterUserDTO user)
        {
            ServiceResult<UserDTO> result= await _userService.UpdateUser(id, user);
            if (!result.IsSuccess)
                return StatusCode((int)result.ErrorStatus, result.ErrorMessage);
            return Ok(result.Data);

        }

      
    }
}
