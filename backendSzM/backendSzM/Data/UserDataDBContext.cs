using backendSzM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backendSzM.Data
{
    public class  UserDataDBContext:DbContext
    {
        public UserDataDBContext(DbContextOptions<UserDataDBContext> options) : base(options)
        { 
        
        }
        public DbSet<UserData> Users { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }
      
    }
}
