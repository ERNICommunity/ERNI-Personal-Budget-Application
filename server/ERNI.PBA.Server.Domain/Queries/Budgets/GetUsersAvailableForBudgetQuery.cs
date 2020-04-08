using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetUsersAvailableForBudgetQuery : QueryBase<IEnumerable<UserOutputModel>>
    {
        public BudgetTypeEnum BudgetType { get; set; }
    }
}
