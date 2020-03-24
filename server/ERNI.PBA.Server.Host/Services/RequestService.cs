using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Services
{
    public class RequestService : IRequestService
    {
        private readonly IBudgetRepository _budgetRepository;

        public RequestService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<IList<Transaction>> CreateTeamTransactions(int requestId, int userId, decimal amount, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var teamBudgets = await CreateTeamBudgets(requestId, userId, currentYear, amount, cancellationToken);

            var transactions = new List<Transaction>();
            foreach (var teamBudget in teamBudgets)
            {
                var transaction = new Transaction
                {
                    BudgetId = teamBudget.BudgetId,
                    UserId = userId,
                    Amount = teamBudget.Payment
                };
                transactions.Add(transaction);
            }

            return transactions;
        }

        private async Task<IList<TeamBudget>> CreateTeamBudgets(int requestId, int userId, int year, decimal amount, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetTeamBudgets(userId, year, cancellationToken);
            var teamBudgets = budgets.Select(x => new TeamBudget
            {
                BudgetId = x.Id,
                Amount = x.Amount - (x.Transactions?.Where(_ => _.RequestId != requestId).Sum(_ => _.Amount) ?? 0)
            }).ToList();

            var availableFunds = teamBudgets.Sum(_ => _.Amount);
            if (availableFunds < amount)
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Requested amount {amount} exceeds the limit.");

            var payment = amount / budgets.Length;
            foreach (var budget in teamBudgets)
            {
                budget.Payment += Math.Abs(payment);
            }

            RecalculateTeamBudgets(teamBudgets);

            return teamBudgets;
        }

        private void RecalculateTeamBudgets(IList<TeamBudget> teamBudgets)
        {
            if (teamBudgets.All(_ => _.AvailableFunds >= 0))
                return;

            var budgetsInDebt = teamBudgets.Where(_ => _.AvailableFunds < 0).ToList();
            var payment = budgetsInDebt.Sum(_ => _.AvailableFunds) / budgetsInDebt.Count;
            foreach (var budget in budgetsInDebt)
            {
                budget.Payment = budget.Amount;
            }

            var budgetsToReCalculate = teamBudgets.Where(_ => _.AvailableFunds > 0).ToList();
            foreach (var budget in budgetsToReCalculate)
            {
                budget.Payment += Math.Abs(payment);
            }

            RecalculateTeamBudgets(teamBudgets);
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
