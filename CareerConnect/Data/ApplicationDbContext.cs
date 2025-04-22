using Microsoft.EntityFrameworkCore;
using CareerConnect.Models;

namespace CareerConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}