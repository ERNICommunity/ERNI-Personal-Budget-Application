using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class DeleteBudgetCommand : IDeleteBudgetCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IRequestRepository _requestRepository;

        public DeleteBudgetCommand(IUnitOfWork unitOfWork,
            IBudgetRepository budgetRepository,
            IRequestRepository requestRepository)
        {
            _unitOfWork = unitOfWork;
            _budgetRepository = budgetRepository;
            _requestRepository = requestRepository;
        }

        public async Task ExecuteAsync(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(parameter, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest,
                    $"Budget with id : {parameter} not id");
            }

            var requestCount = await _requestRepository.GetRequestsCount(parameter, cancellationToken);
            if (requestCount > 0)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, "Cannot delete budget with linked requests");
            }

            _budgetRepository.DeleteBudget(budget);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
