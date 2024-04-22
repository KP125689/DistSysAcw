using System.Threading.Tasks;
using DistSysAcwServer.Models;
using DistSysAcwServer.Auth;
using DistSysAcwServer.Migrations;
using Microsoft.EntityFrameworkCore;



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

    }
}
