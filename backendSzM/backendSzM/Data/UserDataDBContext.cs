using backendSzM.Models;
using Microsoft.EntityFrameworkCore;

namespace backendSzM.Data
{
    public class UserDataDBContext(DbContextOptions<UserDataDBContext> options) : DbContext(options)
    {
        public DbSet<UserData> Users { get; set; }
    }
}
