using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Entities;
using ERNI.PBA.Server.Host.Model.PendingRequests;

namespace ERNI.PBA.Server.Host.Queries.Budgets
{
    public class GetUsersAvailableForBudgetQuery : QueryBase<IEnumerable<UserOutputModel>>
    {
        public BudgetTypeEnum BudgetType { get; set; }
    }
}
