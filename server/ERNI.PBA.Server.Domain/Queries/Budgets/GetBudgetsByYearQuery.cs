using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Output.Budgets;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetBudgetsByYearQuery : QueryBase<IEnumerable<SingleBudgetOutputModel>>
    {
        public int Year { get; set; }
    }
}
