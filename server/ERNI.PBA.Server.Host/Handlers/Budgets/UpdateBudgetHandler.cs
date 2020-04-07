using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Commands.Budgets;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class UpdateBudgetHandler : IRequestHandler<UpdateBudgetCommand, bool>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBudgetHandler(
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(request.Id, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget with id {request.Id} not found");
            }

            budget.Amount = request.Amount;

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
