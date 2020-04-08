using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class InvoiceImageEntityConfiguration : IEntityTypeConfiguration<InvoiceImage>
    {
        public void Configure(EntityTypeBuilder<InvoiceImage> builder)
        {
            builder.ToTable("InvoiceImage");
            builder.Property(_ => _.Name).IsRequired();
            builder.HasKey(_ => _.Id);
            builder.HasOne(_ => _.Request);
        }
    }
}
