using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class BudgetEntityConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.ToTable("Budgets");

            builder.HasOne(b => b.User);
        }
    }
}
