using System.Threading.Tasks;
using DistSysAcwServer.Models;
using DistSysAcwServer.Auth;
using DistSysAcwServer.Migrations;
using Microsoft.EntityFrameworkCore;
using DistSysAcwServer.Controllers;

namespace DistSysAcwServer.Services
{
    public class UserDatabaseAccess
    {

        private readonly UserContext _dbContext;
        //private readonly UserDatabaseAccess _userDatabaseAccess;

        public UserDatabaseAccess(UserContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<string> CreateUser(string username)
        {
            var user = new User
            {
                UserName = username,
                ApiKey = Guid.NewGuid().ToString() // Generate a new GUID as the API key
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.ApiKey;
        }

        public bool CheckUserExists(string ApiKey)
        {
            // Check if a user with the given API key exists in the database
            return _dbContext.Users.Any(u => u.ApiKey == ApiKey);
        }

        public bool CheckUserExists(string ApiKey, string username)
        {
            // Check if a user with the given API key and username exists in the database
            return _dbContext.Users.Any(u => u.ApiKey == ApiKey && u.UserName == username);
        }

        public User GetUser(string ApiKey)
        {
            // Get the user with the given API key from the database
            return _dbContext.Users.FirstOrDefault(u => u.ApiKey == ApiKey);
        }

      
        public async Task<User> GetUserByApiKey(string ApiKey)
        {
            // Your database query logic to retrieve user by API key
            return await _dbContext.users.FirstOrDefaultAsync(u => u.ApiKey == ApiKey);
        }

        public bool DeleteUser(string ApiKey) //task 7
        {
            var user = _dbContext.users.FirstOrDefault(u => u.ApiKey == ApiKey);

            if (user != null)
            {

                _dbContext.users.Remove(user);
                _dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }


        }

        public async Task ArchiveLog(User user, string logString)
        {
            var logArchive = new LogArchives
            {
                Id = user.Id,
                LogString = logString,
                LogDateTime = DateTime.Now
            };

            _dbContext.logs.Add(logArchive);
            await _dbContext.SaveChangesAsync();
        }



    }
}
