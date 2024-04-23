using DistSysAcwServer.Models;
using DistSysAcwServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


#region Task4
namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _dbContext;
        private readonly UserDatabaseAccess _userDatabaseAccess;

        public UserController(UserContext dbContext, UserDatabaseAccess userDatabaseAccess)
        {
            _dbContext = dbContext;
            _userDatabaseAccess = userDatabaseAccess;

        }




        #region Task7
        [HttpDelete("RemoveUser")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> RemoveUser(string username)
        {
            // Get API Key from header
            if (!Request.Headers.TryGetValue(username, out var ApiKeyHeaderValues))
            {
                return BadRequest("ApiKey header is missing.");
            }

            string apiKey = ApiKeyHeaderValues.FirstOrDefault();

            // Check if the API Key is valid
            var user = await _userDatabaseAccess.GetUserByApiKey(apiKey);
            if (user == null)
            {
                return BadRequest("Invalid ApiKey.");
            }

            // Check if the provided username matches the authenticated user
            if (user.UserName != username)
            {
                return BadRequest("Unauthorized. Username does not match the authenticated user.");
            }

            // Remove the user from the database
            var removed = _userDatabaseAccess.DeleteUser(apiKey); 
            if (removed)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
        #endregion Task 7


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
            string apiKey = Guid.NewGuid().ToString();
           
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


         #region Task3
        public interface IUserService
        {
            string CreateUser(string username);
            bool UserExists(string apiKey);
            bool UserExists(string apiKey, string username);
            User GetUserByApiKey(string apiKey);
            void DeleteUser(string apiKey);
        }

        public class UserService : ControllerBase
        {
            UserContext _dbContext;

            public UserService(UserContext dbContext)
            {
                _dbContext = dbContext;
            }

            
            protected async Task<User> CreateUser(string userName)
            {
                var user = new User
                {
                    ApiKey = Guid.NewGuid().ToString(),
                    UserName = userName,
                    Role = "User" // Default role, assuming Role is an enum
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            

            protected async Task<bool> UserExists(string apiKey)
            {
                return await _dbContext.Users.AnyAsync(u => u.ApiKey == apiKey);
            }

            protected async Task<bool> UserExists(string apiKey, string userName)
            {
                return await _dbContext.Users.AnyAsync(u => u.ApiKey == apiKey && u.UserName == userName);
            }

            protected async Task<User> GetUser(string apiKey)
            {
                return await _dbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
            }

            protected async Task<bool> DeleteUser(string apiKey)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
                if (user == null)
                {
                    return false;
                }
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
        #endregion
        // POST: api/user/changerole
        [HttpPost("changerole")]
        [Authorize(Roles = "Admin")] // Ensure only Admin users can access this endpoint
        public IActionResult ChangeRole([FromBody] UserRoleChangeModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Role))
            {
                return BadRequest("Invalid request. Please provide both username and role in the request body.");
            }

            // Find the user by username
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (user == null)
            {
                return NotFound($"User '{model.Username}' not found.");
            }

            // Update user role
            user.Role = model.Role;
            _dbContext.SaveChanges();

            return Ok($"Role updated successfully for user '{model.Username}'. New role: '{model.Role}'.");
            
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                // Log user not found
                LogNotFound($"User with ID {id} not found");
                return NotFound();
            }

            // Log successful request
            LogRequest($"GetUser: User with ID {id} retrieved");
            return Ok(user);
        }


        private void LogRequest(string message)
        {
            var user = GetUserFromRequest();
            if (user != null)
            {
                user.ArchivedLogs.Add(new LogArchives());
                _dbContext.SaveChanges();
            }
        }

        private void LogNotFound(string message)
        {
            var user = GetUserFromRequest();
            if (user != null)
            {
                user.ArchivedLogs.Add(new LogArchives());
                _dbContext.SaveChanges();
            }
        }

        private User GetUserFromRequest()
        {
            // Get the API key from the request header
            if (HttpContext.Request.Headers.TryGetValue("ApiKey", out var apiKeyHeader))
            {
                var apiKey = apiKeyHeader.FirstOrDefault();

                // Find the user with the given API key in the database
                var user = _dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey);

                return user;
            }

            // If API key is not found or user is not authenticated, return null
            return null;
        }
    }
    public class UserRoleChangeModel
    {
        public string? Username { get; set; }
        public string? Role { get; set; }
    }


}


#endregion

