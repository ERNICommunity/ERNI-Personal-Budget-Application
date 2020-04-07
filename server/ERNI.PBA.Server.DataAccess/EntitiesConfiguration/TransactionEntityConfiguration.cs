using ERNI.PBA.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Budget)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => new { x.BudgetId })
                .HasPrincipalKey(x => new { x.Id })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => new { x.UserId })
                .HasPrincipalKey(x => new { x.Id })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
