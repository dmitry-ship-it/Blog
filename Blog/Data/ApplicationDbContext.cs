using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.Data.DbModels;

namespace Blog.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>(entity => entity.ToTable(name: "Article"));
            modelBuilder.Entity<Comment>(entity => entity.ToTable(name: "Comment"));
            modelBuilder.Entity<User>(entity => entity.ToTable(name: "User"));
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
