using System;
using System.Collections.Generic;
using System.Linq;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.Host.Utils;

namespace ERNI.PBA.Server.Host.Services
{
    public static class TransactionCalculator
    {
        public static IList<Transaction> Calculate(IEnumerable<TeamBudget> budgets, decimal distributedAmount)
        {
            var transactions = new List<Transaction>();
            var availableBudgets = new Queue<TeamBudget>(budgets.OrderBy(x => x.Amount));
            var amount = distributedAmount;
            while (availableBudgets.Any())
            {
                var amountPerItem = amount / availableBudgets.Count;

                var first = availableBudgets.Dequeue();
                var amountToDeduct = Math.Min(amountPerItem, first.Amount).Round();

                transactions.Add(new Transaction
                {
                    BudgetId = first.BudgetId,
                    UserId = first.UserId,
                    Amount = amountToDeduct
                });

                amount -= amountToDeduct;
            }

            return transactions;
        }

        private static decimal Round(this decimal payment)
        {
            return Math.Floor(payment * 100) / 100;
        }
    }
}
