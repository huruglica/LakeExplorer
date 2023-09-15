using LakeXplorer.Models;
using Microsoft.EntityFrameworkCore;

namespace LakeXplorer.Data
{
    public class LakeXplorerDbContext : DbContext
    {
        public LakeXplorerDbContext(DbContextOptions<LakeXplorerDbContext> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Lake> Lake { get; set; }
        public DbSet<LakeSighting> LakeSighting { get; set; }
        public DbSet<Like> Like { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>()
                .HasKey(x => new { x.UserId, x.LakeSightingId });
        }
    }
}
