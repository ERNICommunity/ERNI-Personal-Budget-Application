using ERNI.PBA.Server.DataAccess.EntitiesConfiguration;
using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess
{
    public class DatabaseContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Budget> Budgets => Set<Budget>();

        public DbSet<RequestCategory> RequestCategories => Set<RequestCategory>();

        public DbSet<Request> Requests => Set<Request>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<InvoiceImage> InvoiceImages => Set<InvoiceImage>();

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