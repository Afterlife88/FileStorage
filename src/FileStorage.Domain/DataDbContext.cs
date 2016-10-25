using FileStorage.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Domain
{
    public class DataDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ShareEmail> ShareEmails { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FileVersion>().HasIndex(r => r.MD5Hash).IsUnique();
            base.OnModelCreating(builder);
        }
    }
}
