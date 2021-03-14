using Autofac;
using ERNI.PBA.Server.Business;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.Host.Filters;

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

        private static void RegisterServices(ContainerBuilder builder) =>
            builder.RegisterType<ApiExceptionFilter>().AsSelf().InstancePerDependency();
    }
}
