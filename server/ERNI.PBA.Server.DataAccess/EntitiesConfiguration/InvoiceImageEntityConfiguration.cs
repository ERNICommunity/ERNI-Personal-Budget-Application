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
            builder.Property(_ => _.Filename).IsRequired();
            builder.Property(_ => _.BlobPath).IsRequired();
            builder.Property(_ => _.MimeType).IsRequired();
            builder.HasKey(_ => _.Id);
            builder.HasOne(_ => _.Request);
        }
    }
}
