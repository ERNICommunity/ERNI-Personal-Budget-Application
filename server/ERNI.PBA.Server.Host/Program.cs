using System;
using ERNI.PBA.Server.DataAccess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
                var host = BuildWebHost(args);

                using (var serviceScope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();

                    context.Database.Migrate();
                    DbSeed.Seed(context);
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

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false)

                // .ConfigureLogging(logging =>
                // {
                //    logging.ClearProviders();
                //    logging.SetMinimumLevel(LogLevel.Trace);
                // })
                // .UseNLog()  // NLog: setup NLog for Dependency injection
                .Build();
    }
}
