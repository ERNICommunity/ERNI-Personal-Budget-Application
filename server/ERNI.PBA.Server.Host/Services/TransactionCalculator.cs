using System;
using System.Collections.Generic;
using System.Linq;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.Host.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Services
{
    public class TransactionCalculator
    {
        private readonly IEnumerable<Budget> _budgets;
        private readonly int _userId;

        private IList<TeamBudget> _teamBudgets;

        public TransactionCalculator(IEnumerable<Budget> budgets, int userId)
        {
            _budgets = budgets;
            _userId = userId;
        }

        public IList<Transaction> Transactions { get; private set; }

        public void Calculate(decimal amount)
        {
            Calculate(0, amount);
        }

        public void Calculate(int requestId, decimal amount)
        {
            _teamBudgets = _budgets.Select(x => new TeamBudget
            {
                BudgetId = x.Id,
                Amount = x.Amount - (x.Transactions?.Where(_ => _.RequestId != requestId).Sum(_ => _.Amount) ?? 0)
            }).OrderBy(x => x.Amount).ToList();

            var availableFunds = _teamBudgets.Sum(_ => _.Amount);
            if (availableFunds < amount)
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {amount} exceeds the limit.");

            RecalculateBudgets(amount);
            CentsRounding(amount);

            CreateTransactions();
        }

        private void RecalculateBudgets(decimal totalAmount)
        {
            var amount = totalAmount - _teamBudgets.Sum(x => x.Payment);
            if (amount <= 0.01M)
                return;

            var availableBudgets = _teamBudgets.Where(x => x.AvailableFunds > 0).ToList();

            var first = availableBudgets.First();
            var payment = amount / availableBudgets.Count;
            var maxPayment = payment > first.AvailableFunds ? first.AvailableFunds : payment;

            foreach (var budget in availableBudgets)
            {
                budget.Payment += maxPayment;
            }

            RecalculateBudgets(totalAmount);
        }

        private void CentsRounding(decimal amount)
        {
            foreach (var budget in _teamBudgets)
            {
                budget.Payment = Math.Floor(budget.Payment * 100) / 100;
            }

            var cents = amount - _teamBudgets.Sum(x => x.Payment);
            if (cents > 0)
            {
                const decimal cent = 0.01M;

                while (cents > 0 && _teamBudgets.Any(x => x.AvailableFunds > 0))
                {
                    foreach (var budget in _teamBudgets)
                    {
                        if (budget.AvailableFunds > 0)
                        {
                            budget.Payment += cent;
                            cents -= cent;
                            if (cents <= 0)
                                break;
                        }
                    }
                }
            }
        }

        private void CreateTransactions()
        {
            if (_teamBudgets == null)
                throw new InvalidOperationException("Calculation not initialize");

            Transactions = _teamBudgets.Select(x => new Transaction
            {
                BudgetId = x.BudgetId,
                UserId = _userId,
                Amount = x.Payment
            }).ToList();
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
