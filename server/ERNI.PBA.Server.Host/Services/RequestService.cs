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
        private readonly ITeamRequestRepository _teamRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(
            IBudgetRepository budgetRepository,
            ITeamRequestRepository teamRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _teamRequestRepository = teamRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamRequest> CreateTeamRequests(int userId, TeamRequestInputModel requestInputModel, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;
            var teamBudgets = await CreateTeamBudgets(userId, requestInputModel, cancellationToken);
            if (!teamBudgets.Any())
                return null;

            var requests = new List<Request>();
            foreach (var teamBudget in teamBudgets)
            {
                var request = new Request
                {
                    BudgetId = teamBudget.BudgetId,
                    UserId = teamBudget.UserId,
                    Year = currentYear,
                    Title = requestInputModel.Title,
                    Amount = teamBudget.Payment,
                    Date = requestInputModel.Date.ToLocalTime(),
                    State = RequestState.Pending
                };
                requests.Add(request);
            }

            var teamRequest = new TeamRequest
            {
                UserId = userId,
                Title = requestInputModel.Title,
                Year = requestInputModel.Year,
                Date = requestInputModel.Date.ToLocalTime(),
                State = RequestState.Pending,
                Requests = requests
            };

            return teamRequest;
        }

        private async Task<IList<TeamBudget>> CreateTeamBudgets(int userId, TeamRequestInputModel requestInputModel, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetTeamBudgets(userId, requestInputModel.Year, cancellationToken);
            var teamBudgets = budgets.Select(x => new TeamBudget
            {
                BudgetId = x.Id,
                UserId = x.UserId,
                Amount = x.Amount - (x.Requests?.Where(_ => _.TeamRequestId != requestInputModel.RequestId).Sum(_ => _.Amount) ?? 0)
            }).ToList();

            var availableFunds = teamBudgets.Sum(_ => _.Amount);
            if (availableFunds < requestInputModel.Amount)
                throw new NoAvailableFundsException();

            var payment = requestInputModel.Amount / budgets.Length;
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

            public int UserId { get; set; }

            public decimal Amount { get; set; }

            public decimal Payment { get; set; }

            public decimal AvailableFunds => Amount - Payment;
        }
    }
}
