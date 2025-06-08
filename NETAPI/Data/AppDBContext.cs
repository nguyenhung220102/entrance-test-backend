//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NETAPI.Models;

namespace NETAPI.Data
{
    public class AppDbContext : DbContext //: IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Change table name to map with the actual database tables
            builder.Entity<User>().ToTable("users");
            builder.Entity<Token>().ToTable("token");


            builder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(32);
                entity.Property(u => u.LastName).HasMaxLength(32);
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.UpdatedAt).IsRequired();
            });

            builder.Entity<Token>(entity =>
            {
                entity.Property(t => t.RefreshToken).HasMaxLength(250).IsRequired();
                entity.Property(t => t.ExpiresIn).HasMaxLength(64).IsRequired();
            });
        }
    }
}