using DistSysAcwServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistSysAcwServer.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// This DbContext contains the database context defined in UserContext.cs
        /// You can use it inside your controllers to perform database CRUD functionality
        /// </summary>
        protected Models.UserContext DbContext { get; set; }
        /// <summary>
        /// Abstract base controller containing a dependency-injected scoped DbContext (UserContext).
        /// Inherit from this abstract base to access the common DbContext in a controller.
        /// </summary>
        /// <param name="dbcontext">Dependency-injected DbContext</param>
        public BaseController(Models.UserContext dbcontext)
        {
            DbContext = dbcontext;
        }

        #region Task3
        public interface IUserService
        {
            /* string CreateUser(string username);
            bool UserExists(string apiKey);
            bool UserExists(string apiKey, string username);
            User GetUserByApiKey(string apiKey);
            void DeleteUser(string apiKey);*/
        }

        public class UserService : IUserService
        {
            UserContext _dbContext;

            public UserService(UserContext dbContext)
            {
                _dbContext = dbContext;
            }

            public string CreateUser(string username)
            {

                var user = new User
                {
                    ApiKey = Guid.NewGuid().ToString(),
                    UserName = username,
                    Role = "User" // Default role
                };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                return user.ApiKey;
            }

            public bool UserExists(string apiKey) //delete all n simplify and follow lab 
            {
                return _dbContext.Users.Any(u => u.ApiKey == apiKey);
            }

            public bool UserExists(string apiKey, string username)
            {
                return _dbContext.Users.Any(u => u.ApiKey == apiKey && u.UserName == username);
            }

            public User GetUserByApiKey(string apiKey)
            {
                return _dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey)!;
            }

            public void DeleteUser(string apiKey)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey);
                if (user != null)
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                }
            }
        }
        #endregion
    }
}