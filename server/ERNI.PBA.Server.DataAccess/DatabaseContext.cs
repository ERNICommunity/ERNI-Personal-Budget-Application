using ERNI.PBA.Server.DataAccess.EntitiesConfiguration;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<InvoiceImage> InvoiceImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BudgetEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RequestCategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RequestEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionEntityConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceImageEntityConfiguration());
        }
    }
}
