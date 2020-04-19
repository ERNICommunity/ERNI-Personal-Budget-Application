using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets
{
    public interface IGetUsersAvailableForBudgetQuery : IQuery<BudgetTypeEnum, IEnumerable<UserOutputModel>>
    {
    }
}
