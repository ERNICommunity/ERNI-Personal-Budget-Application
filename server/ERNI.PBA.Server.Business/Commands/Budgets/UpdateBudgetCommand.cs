using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class UpdateBudgetCommand : Command<UpdateBudgetRequest, bool>, IUpdateBudgetCommand
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBudgetCommand(
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<bool> Execute(UpdateBudgetRequest parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(parameter.Id, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget with id {parameter.Id} not found");
            }

            budget.Amount = parameter.Amount;

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
