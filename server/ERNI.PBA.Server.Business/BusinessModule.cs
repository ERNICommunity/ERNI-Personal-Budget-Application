using System.Reflection;
using Autofac;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using Module = Autofac.Module;

namespace ERNI.PBA.Server.Business
{
    public class BusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterServices(builder);
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(ICommand<,>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(ICommand<>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(IQuery<,>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}
