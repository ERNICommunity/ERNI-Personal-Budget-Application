using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class TeamRequestEntityConfiguration : IEntityTypeConfiguration<TeamRequest>
    {
        public void Configure(EntityTypeBuilder<TeamRequest> builder)
        {
            builder.ToTable("TeamRequests");

            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.UserId).IsRequired();
            builder.Property(_ => _.Date).IsRequired();

            builder.HasMany(_ => _.Requests)
                .WithOne(_ => _.TeamRequest)
                .HasForeignKey(_ => _.TeamRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
