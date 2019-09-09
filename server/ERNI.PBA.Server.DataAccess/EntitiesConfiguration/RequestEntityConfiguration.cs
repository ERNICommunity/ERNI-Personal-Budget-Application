using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class RequestEntityConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Requests");

            builder.HasKey(x => x.Id);
            //builder.Property(x => x.Year).IsRequired();
            //builder.Property(x => x.CategoryId).IsRequired();
            //builder.Property(x => x.Url).IsRequired().HasMaxLength(250);
            //builder.Property(x => x.UserId).IsRequired();
            //builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            //builder.Property(x => x.Amount).IsRequired();
            //builder.Property(x => x.Date).IsRequired();
            //builder.Property(x => x.State).IsRequired();

            builder.HasOne(b => b.Budget)
                .WithMany()
                .HasForeignKey(r => new { r.BudgetId })
                .HasPrincipalKey(b => new { b.Id })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(_ => _.Category).WithMany();
        }
    }
}
