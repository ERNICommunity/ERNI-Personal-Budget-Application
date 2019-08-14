using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERNI.PBA.Server.DataAccess.EntitiesConfiguration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            //builder.HasKey(x => x.Id);
            //builder.Property(x => x.UniqueIdentifier).IsRequired().HasMaxLength(150);
            //builder.Property(x => x.IsAdmin).IsRequired();
            //builder.Property(x => x.IsSuperior).IsRequired();
            //builder.Property(x => x.FirstName).IsRequired().HasMaxLength(150);
            //builder.Property(x => x.LastName).IsRequired().HasMaxLength(150);
            //builder.Property(x => x.Username).IsRequired().HasMaxLength(150);
            //builder.Property(x => x.State).IsRequired();
        }
    }
}
