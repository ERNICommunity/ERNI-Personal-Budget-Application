using Autofac;
using ERNI.PBA.Server.Business;
using ERNI.PBA.Server.Business.Commands.Users;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.Host.Filters;
using Azure.Identity;
using Microsoft.Graph;
using ERNI.PBA.Server.Host.Configuration;
using ERNI.PBA.Server.Graph;

namespace ERNI.PBA.Server.Host
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterModules(builder);
            RegisterServices(builder);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<BusinessModule>();
            builder.RegisterModule<DataAccessModule>();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var config = c.Resolve<MicrosoftGraphConfiguration>();
                return new GraphServiceClient(new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret));
            }).SingleInstance();

            builder.RegisterType<GraphFacade>().AsSelf().InstancePerDependency();

            builder.RegisterType<ApiExceptionFilter>().AsSelf().InstancePerDependency();

            builder.RegisterType<SyncUserObjectIdCommand>().AsSelf().InstancePerDependency();
        }
    }
}
