using Microsoft.EntityFrameworkCore;
using NetCoreExercise.Models;

namespace NetCoreExercise.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}