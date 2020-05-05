using System;
using System.Diagnostics;
using ERNI.PBA.Server.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public class DatabaseFacade
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseFacade(IServiceProvider serviceProvider, string databaseName)
        {
            DatabaseName = databaseName;
            _serviceProvider = serviceProvider;

            Debug.WriteLine($"Initializing database facade. Database name: {databaseName}");
        }

        public string DatabaseName { get; }

        public bool Delete()
        {
            var singleuserSql = $@"ALTER DATABASE {DatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            var dropDatabaseSql = $@"DROP DATABASE {DatabaseName}";

            try
            {
                ExecuteOnMaster(c =>
                {
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
#pragma warning disable 618
                    c.Database.ExecuteSqlCommand(singleuserSql);
                    c.Database.ExecuteSqlCommand(dropDatabaseSql);
#pragma warning restore 618
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
                });
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void ExecuteOnMaster(Action<DbContext> action)
        {
            string masterConnection;
            using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var cnn = context.Database.GetDbConnection().ConnectionString;
                masterConnection = cnn.Replace(DatabaseName, "master");
            }

            var options = new DbContextOptionsBuilder().UseSqlServer(masterConnection).Options;
            using (var master = new DbContext(options))
            {
                action(master);
            }
        }

        public void Initialize()
        {
            using (var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                context.Database.Migrate();
            }
        }
    }
}
