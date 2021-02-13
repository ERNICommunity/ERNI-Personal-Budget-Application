using System;
using System.Threading;
using Autofac.Extensions.DependencyInjection;
using ERNI.PBA.Server.Business.Queries.Employees;
using ERNI.PBA.Server.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ERNI.PBA.Server.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            // var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                // logger.Debug("init main");
                var host = CreateHostBuilder(args).Build();

                using (var serviceScope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();

                    context.Database.Migrate();

                    var cmd = serviceScope.ServiceProvider.GetRequiredService<SyncUserObjectIdCommand>();
                    cmd.Execute().Wait();
                }

                host.Run();
            }
            catch (Exception)
            {
                // logger.Error(ex, "Stopped program because of exception");
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                // NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
