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
        public DbSet<LobbyCon> LobbyCons { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Lobby> Lobbies { get; set; }
        public DbSet<Komment> Komments { get; set; }

        }
        
        
        
      
    }

