using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public User()
        {
           ApiKey = Guid.NewGuid().ToString();
          

        } 

        public virtual ICollection<LogArchives> ArchivedLogs { get; set; }


        [Key]
        [Required]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public int Id { get; internal set; }
        [NotMapped]
        public object PrivateRSAKey { get; internal set; }

        #endregion
    }


    #region Task13?
    // TODO: You may find it useful to add code here for Logging

    public class LogArchives
    {
        public int LogArchiveId { get; set; }
        public int Id { get; set; } // Foreign key to the User table
        public string LogString { get; set; }
        public DateTime LogDateTime { get; set; }
    }
    #endregion


}