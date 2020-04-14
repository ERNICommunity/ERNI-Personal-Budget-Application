using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Payloads;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets
{
    public interface IGetBudgetLeftQuery : IQuery<BudgetLeftModel, UserModel[]>
    {
    }
}
