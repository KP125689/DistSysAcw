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

        public UserDatabaseAccess(UserContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<User> GetUserByApiKey(string apiKey)
        {
            // Your database query logic to retrieve user by API key
            // Assuming you have a DbSet<User> called Users in your UserContext
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);
        }

        public bool DeleteUser(string apiKey) //task 7
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey);
            if (user != null)
            {

                _dbContext.Users.Remove(user);
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
                UserId = user.Id,
                LogString = logString,
                LogDateTime = DateTime.Now
            };

            _dbContext.logs.Add(logArchive);
            await _dbContext.SaveChangesAsync();
        }



    }
}
