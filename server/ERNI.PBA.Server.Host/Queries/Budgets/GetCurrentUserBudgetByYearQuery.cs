using System.Collections.Generic;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Queries.Budgets
{
    public class GetCurrentUserBudgetByYearQuery : QueryBase<IEnumerable<BudgetOutputModel>>
    {
        public int UserId { get; set; }

        public int Year { get; set; }
    }
}
