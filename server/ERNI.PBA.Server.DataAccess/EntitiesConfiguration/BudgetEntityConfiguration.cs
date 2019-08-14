using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class BudgetEntityConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.ToTable("Budgets");

            builder.HasKey(x => new { x.UserId, x.Year });
            //builder.Property(x => x.Year).IsRequired();
            //builder.Property(x => x.UserId).IsRequired();
            //builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(b => b.User);
        }
    }
}
