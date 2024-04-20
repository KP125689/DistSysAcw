using DistSysAcwServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


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

        // GET: api/user/new
        [HttpGet("new")]
        public IActionResult NewGet(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { errors = new { username = new[] { "The username field is required." } } });
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

        // POST: api/user/new
        [HttpPost("new")]
        public IActionResult NewPost([FromBody] CreateUserModel model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState });
            }
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (existingUser != null)
            {
                return StatusCode(403, "Oops. This username is already in use. Please try again with a new username.");
            }

            var user = new User
            {
                UserName = model.Username,
                ApiKey = Guid.NewGuid().ToString(),
                Role = "User" // Assign a default role for new users
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return Ok(user.ApiKey);
        }

        public class CreateUserModel
        {
            [Required(ErrorMessage = "The username field is required.")]
            public string? Username { get; set; }
        }


    }

}
#endregion