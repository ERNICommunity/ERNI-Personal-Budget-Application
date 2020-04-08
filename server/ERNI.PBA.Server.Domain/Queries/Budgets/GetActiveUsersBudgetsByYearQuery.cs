using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetActiveUsersBudgetsByYearQuery : QueryBase<IEnumerable<BudgetOutputModel>>
    {
        public int Year { get; set; }
    }
}
