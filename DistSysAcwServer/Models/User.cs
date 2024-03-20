using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DistSysAcwServer.Models
{
    /// <summary>
    /// User data class
    /// </summary>
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key
        
        public User() { } // Empty constructor

        [Key]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        #endregion
    }

    #region Task3
    public interface IUserService
    {
        string CreateUser(string username);
        bool UserExists(string apiKey);
        bool UserExists(string apiKey, string username);
        User? GetUserByApiKey(string apiKey);
        void DeleteUser(string apiKey);
    }

    public class UserService : IUserService
    {
        private readonly UserContext _dbContext;

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

        public bool UserExists(string apiKey)
        {
            return _dbContext.Users.Any(u => u.ApiKey == apiKey);
        }

        public bool UserExists(string apiKey, string username)
        {
            return _dbContext.Users.Any(u => u.ApiKey == apiKey && u.UserName == username);
        }

        public User? GetUserByApiKey(string apiKey)
        {
            return _dbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey);
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





    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion


}