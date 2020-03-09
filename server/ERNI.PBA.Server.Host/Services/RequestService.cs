using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Services
{
    public class RequestService : IRequestService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Transaction[]> CreateTeamTransactions(int userId, PostRequestModel postRequestModel, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var teamBudgets = await CreateTeamBudgets(userId, currentYear, postRequestModel, cancellationToken);
            if (!teamBudgets.Any())
                return null;

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

            return transactions.ToArray();
        }

        private async Task<IList<TeamBudget>> CreateTeamBudgets(int userId, int year, PostRequestModel postRequestModel, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetTeamBudgets(userId, year, cancellationToken);
            var teamBudgets = budgets.Select(x => new TeamBudget
            {
                BudgetId = x.Id,
                Amount = x.Amount - (x.Transactions?.Sum(_ => _.Amount) ?? 0)
            }).ToList();

            var availableFunds = teamBudgets.Sum(_ => _.Amount);
            if (availableFunds < postRequestModel.Amount)
                throw new NoAvailableFundsException();

            var payment = postRequestModel.Amount / budgets.Length;
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
