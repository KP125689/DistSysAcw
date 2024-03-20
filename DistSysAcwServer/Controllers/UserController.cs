using DistSysAcwServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


#region Task4
namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _dbContext;

        public UserController(UserContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("New")]
        public IActionResult NewGet(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }

            var userExists = _dbContext.Users.Any(u => u.UserName == username);
            if (userExists)
            {
                return Ok("True - User Does Exist! Did you mean to do a POST to create a new user?");
            }
            else
            {
                return Ok("False - User Does Not Exist! Did you mean to do a POST to create a new user?");
            }
        }

        [HttpPost("New")]
        public IActionResult NewPost([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (existingUser != null)
            {
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }

            var user = new User
            {
                UserName = username,
                ApiKey = Guid.NewGuid().ToString(),
                Role = _dbContext.Users.Any() ? "User" : "Admin" // If it's the first user, set as Admin
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return Ok(user.ApiKey);
        }
    }
        
}
#endregion