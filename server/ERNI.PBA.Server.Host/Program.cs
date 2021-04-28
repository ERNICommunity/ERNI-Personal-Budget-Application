using Autofac.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using ERNI.PBA.Server.Business.Queries.Employees;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ERNI.PBA.Server.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var blobService = serviceScope.ServiceProvider.GetRequiredService<BlobServiceClient>();
                var blobStorageSettings = serviceScope.ServiceProvider.GetRequiredService<IOptions<BlobStorageSettings>>();

                context.Database.Migrate();

                var exists = blobService.GetBlobContainerClient(blobStorageSettings.Value.AttachmentDataContainerName).Exists();
                if (!exists)
                {
                    blobService.CreateBlobContainer(blobStorageSettings.Value.AttachmentDataContainerName);
                }
                var cmd = serviceScope.ServiceProvider.GetRequiredService<SyncUserObjectIdCommand>();
                cmd.Execute().Wait();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}