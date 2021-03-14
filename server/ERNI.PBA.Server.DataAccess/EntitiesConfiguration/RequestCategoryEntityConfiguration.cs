using ERNI.PBA.Server.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class RequestCategoryEntityConfiguration : IEntityTypeConfiguration<RequestCategory>
    {
        public void Configure(EntityTypeBuilder<RequestCategory> builder) => builder.ToTable("RequestCategories");
        //builder.HasKey(x => x.Id);
        //builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        //builder.Property(x => x.IsActive).IsRequired();
        //builder.Property(x => x.IsUrlNeeded).IsRequired();
    }
}
