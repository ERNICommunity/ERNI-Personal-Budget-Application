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
            var teamBudgets = budgets.OrderBy(x => x.Amount).ToList();

            var amount = distributedAmount;
            var availableBudgets = new Queue<TeamBudget>(teamBudgets);
            while (availableBudgets.Any())
            {
                var amountPerItem = PaymentRounding(amount / availableBudgets.Count);

                var first = availableBudgets.Dequeue();
                var amountToDeduct = Math.Min(amountPerItem, first.Amount);

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

        private static decimal PaymentRounding(decimal payment)
        {
            return Math.Floor(payment * 100) / 100;
        }
    }
}
