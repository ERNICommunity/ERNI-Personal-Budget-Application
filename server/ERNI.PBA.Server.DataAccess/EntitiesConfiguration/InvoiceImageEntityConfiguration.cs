using System;
using System.Collections.Generic;
using System.Text;
using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class InvoiceImageEntityConfiguration : IEntityTypeConfiguration<InvoiceImage>
    {
        public void Configure(EntityTypeBuilder<InvoiceImage> builder)
        {
            builder.ToTable("InvoiceImage");
            builder.Property(_ => _.Name).IsRequired();
            builder.Property(_ => _.Extension).IsRequired();
            builder.HasKey(_ => _.Id);
            builder.HasOne(_ => _.Request);
        }
    }
}
