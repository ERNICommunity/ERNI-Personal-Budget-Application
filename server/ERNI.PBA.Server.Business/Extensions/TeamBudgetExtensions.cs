﻿using System;
using System.Collections.Generic;
using System.Linq;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Business.Extensions
{
    public static class TeamBudgetExtensions
    {
        public static IList<TeamBudget> ToTeamBudgets(this IEnumerable<Budget> budgets) => budgets.ToTeamBudgets(x => true);

        public static IList<TeamBudget> ToTeamBudgets(this IEnumerable<Budget> budgets, Func<Transaction, bool> predicate) =>
            budgets.Select(_ => new TeamBudget
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
