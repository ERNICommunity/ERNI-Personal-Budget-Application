﻿using ERNI.PBA.Server.DataAccess.Model;
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

            builder.HasOne(b => b.Budget)
                .WithMany(b => b.Requests)
                .HasForeignKey(r => new { r.BudgetId })
                .HasPrincipalKey(b => new { b.Id })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(_ => _.Category).WithMany();
        }
    }
}
