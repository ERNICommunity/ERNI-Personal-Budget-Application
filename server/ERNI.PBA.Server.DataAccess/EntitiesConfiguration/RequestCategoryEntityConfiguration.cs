using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class RequestCategoryEntityConfiguration : IEntityTypeConfiguration<RequestCategory>
    {
        public void Configure(EntityTypeBuilder<RequestCategory> builder)
        {
            builder.ToTable("RequestCategories");
        }
    }
}
