using Microsoft.EntityFrameworkCore;

namespace DistSysAcwServer.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base() {}
        
        public DbSet<User> Users { get; set; }

        //TODO: Task13
        public DbSet<User> users { get; set; }
        
        public DbSet<LogArchives> logs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DistSysAcw;");
        }
    }
}