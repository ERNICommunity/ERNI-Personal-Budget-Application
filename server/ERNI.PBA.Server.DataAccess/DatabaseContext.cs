using ERNI.PBA.Server.DataAccess.EntitiesConfiguration;
using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERNI.PBA.Server.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<RequestCategory> RequestCategories { get; set; }

        public DbSet<Request> Requests { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BudgetEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RequestCategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RequestEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        }
    }
}
