using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Responses.Budgets;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets
{
    public interface IGetBudgetsByYearQuery : IQuery<int, IEnumerable<SingleBudgetOutputModel>>
    {
    }
}
