using Autofac;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.DataAccess
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterServices(builder);
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            builder.RegisterType<BudgetRepository>().As<IBudgetRepository>().InstancePerDependency();
            builder.RegisterType<RequestRepository>().As<IRequestRepository>().InstancePerDependency();
            builder.RegisterType<RequestCategoryRepository>().As<IRequestCategoryRepository>().InstancePerDependency();
            builder.RegisterType<InvoiceImageRepository>().As<IInvoiceImageRepository>().InstancePerDependency();
        }
    }
}
