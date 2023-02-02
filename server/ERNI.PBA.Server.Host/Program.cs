using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using ClosedXML.Excel;
using ClosedXML.Graphics;
using ERNI.PBA.Server.Business.Commands.Users;
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

            // Fonts expected by ClosedXML are not available on Linux
            // we bundle Calibri with the app instead
            using (var fallbackFontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ERNI.PBA.Server.Host.Resources.carlito.Carlito-Regular.ttf"))
            {
                LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateWithFontsAndSystemFonts(fallbackFontStream);
            }

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