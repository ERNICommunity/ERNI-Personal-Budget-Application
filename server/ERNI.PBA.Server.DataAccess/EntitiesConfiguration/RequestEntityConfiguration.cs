﻿using ERNI.PBA.Server.Domain.Models.Entities;
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

            builder.HasOne(_ => _.Category).WithMany();
            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Request)
                .HasForeignKey(x => new { x.RequestId, x.RequestType })
                .HasPrincipalKey(x => new { x.Id, x.RequestType })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
