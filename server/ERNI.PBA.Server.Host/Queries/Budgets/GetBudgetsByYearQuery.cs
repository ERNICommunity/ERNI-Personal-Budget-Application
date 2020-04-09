using System.Collections.Generic;
using ERNI.PBA.Server.Host.Model.Budgets;

namespace ERNI.PBA.Server.Host.Queries.Budgets
{
    public class GetBudgetsByYearQuery : QueryBase<IEnumerable<SingleBudgetOutputModel>>
    {
        public int Year { get; set; }
    }
}
