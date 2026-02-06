using backendSzM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backendSzM.Data
{
    public class  UserDataDBContext (DbContextOptions<UserDataDBContext> options) : DbContext(options)
    {
        
        public DbSet<UserData> Users { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }
        public DbSet<LobbyCon> LobbyCons { get; set; }
        public DbSet<UserAuth> Auths { get; set; }
        public DbSet<Lobby> Lobbies { get; set; }
    }
        
        
        
      
    }

