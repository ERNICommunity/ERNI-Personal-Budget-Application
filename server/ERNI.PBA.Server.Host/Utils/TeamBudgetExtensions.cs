using System;
using System.Collections.Generic;
using System.Linq;
using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Host.Utils
{
    public static class TeamBudgetExtensions
    {
        public static IList<TeamBudget> ToTeamBudgets(this IEnumerable<Budget> budgets)
        {
            return budgets.ToTeamBudgets(x => true);
        }

        public static IList<TeamBudget> ToTeamBudgets(this IEnumerable<Budget> budgets, Func<Transaction, bool> predicate)
        {
            return budgets.Select(_ => new TeamBudget
            {
                BudgetId = _.Id,
                UserId = _.UserId,
                Amount = _.Amount - _.Transactions
                             .Where(x => x.Request.State != RequestState.Rejected)
                             .Where(predicate)
                             .Sum(x => x.Amount)
            }).ToList();
        }
    }
}
