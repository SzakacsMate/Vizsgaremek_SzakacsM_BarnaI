using backendSzM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backendSzM.Data
{
    public class UserDataDBContext : DbContext
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
        public DbSet<Location> Locations { get; set; }
        public DbSet<Rep> Reps { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Komment>()
        .HasOne(k => k.FogadoUser)
        .WithMany(u => u.Komments)
        .HasForeignKey(k => k.FogadoUserId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Komment>()
                .HasOne(k => k.KommenteloUser)
                .WithMany() 
                .HasForeignKey(k => k.KommentaloUserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Rep>()
        .HasOne(k => k.RepFogadoUser)
        .WithMany(u => u.Reps)
        .HasForeignKey(k => k.RepFogadoUserId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rep>()
                .HasOne(k => k.RepAdoUser)
                .WithMany()
                .HasForeignKey(k => k.RepAdoUserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }






    }
      
    }

