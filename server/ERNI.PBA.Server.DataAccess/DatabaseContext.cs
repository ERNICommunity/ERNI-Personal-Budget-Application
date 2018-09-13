using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<Request> Requests { get; set; }

        public DbSet<RequestCategory> RequestCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<User>();

            var budget = modelBuilder.Entity<Budget>();
            budget.HasOne(b => b.User);
            budget.HasKey(b => new { b.UserId, b.Year });

            var request = modelBuilder.Entity<Request>();
            request.HasOne(b => b.Budget)
                .WithMany()
                .HasForeignKey(r => new { r.UserId, r.Year })
                .HasPrincipalKey(b => new { b.UserId, b.Year }).OnDelete(DeleteBehavior.Restrict);

            request.HasOne(_ => _.Category)
                .WithMany();
        }
    }
}
