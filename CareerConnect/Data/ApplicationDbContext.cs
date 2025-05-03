using Microsoft.EntityFrameworkCore;
using CareerConnect.Models;

namespace CareerConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<CV> CVs => Set<CV>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<Application> Applications => Set<Application>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Message> Messages => Set<Message>();

    }
}