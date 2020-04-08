using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetCurrentUserBudgetByYearQuery : QueryBase<IEnumerable<BudgetOutputModel>>
    {
        public int UserId { get; set; }

        public int Year { get; set; }
    }
}
