using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetActiveUsersBudgetsByYearQuery : QueryBase<IEnumerable<BudgetOutputModel>>
    {
        public int Year { get; set; }
    }
}
