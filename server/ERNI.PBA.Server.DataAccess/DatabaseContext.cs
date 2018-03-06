using ERNI.PBA.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace ERNI.PBA.Server.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
