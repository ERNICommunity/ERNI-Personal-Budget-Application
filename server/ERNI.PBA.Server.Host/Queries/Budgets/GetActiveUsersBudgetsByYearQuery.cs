using System.Collections.Generic;
using ERNI.PBA.Server.Host.Model;
using MediatR;

namespace ERNI.PBA.Server.Host.Queries.Budgets
{
    public class GetActiveUsersBudgetsByYearQuery : QueryBase<IEnumerable<BudgetOutputModel>>, IRequest<IEnumerable<BudgetOutputModel>>
    {
        public int Year { get; set; }
    }
}
