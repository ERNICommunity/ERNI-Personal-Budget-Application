using System;
using System.Collections.Generic;
using System.Linq;
using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Services
{
    public class TransactionCalculator
    {
        private readonly IEnumerable<Budget> _budgets;
        private readonly int _userId;

        public TransactionCalculator(IEnumerable<Budget> budgets, int userId)
        {
            _budgets = budgets;
            _userId = userId;
        }

        public IList<Transaction> Calculate(decimal distributedAmount)
        {
            return Calculate(0, distributedAmount);
        }

        public IList<Transaction> Calculate(int requestId, decimal distributedAmount)
        {
            var teamBudgets = _budgets.Select(x => new TeamBudget
            {
                BudgetId = x.Id,
                Amount = x.Amount - (x.Transactions?.Where(_ => _.RequestId != requestId).Sum(_ => _.Amount) ?? 0)
            }).Where(x => x.AvailableFunds > 0).OrderBy(x => x.Amount).ToList();

            var amount = distributedAmount;
            var availableBudgets = new Queue<TeamBudget>(teamBudgets);
            while (availableBudgets.Any())
            {
                var amountPerItem = PaymentRounding(amount / availableBudgets.Count);

                var first = availableBudgets.Dequeue();
                var amountToDeduct = Math.Min(amountPerItem, first.AvailableFunds);

                first.Payment = amountToDeduct;
                amount -= amountToDeduct;
            }

            return teamBudgets.Select(x => new Transaction
            {
                BudgetId = x.BudgetId,
                UserId = _userId,
                Amount = x.Payment
            }).ToList();
        }

        private decimal PaymentRounding(decimal payment)
        {
            return Math.Floor(payment * 100) / 100;
        }

        private class TeamBudget
        {
            public int BudgetId { get; set; }

            public decimal Amount { get; set; }

            public decimal Payment { get; set; }

            public decimal AvailableFunds => Amount - Payment;
        }
    }
}
