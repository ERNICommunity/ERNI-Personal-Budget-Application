﻿using ERNI.PBA.Server.Domain.Models.Entities;
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
                .HasForeignKey(x => new { x.BudgetId, x.RequestType })
                .HasPrincipalKey(x => new { x.Id, x.BudgetType })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
