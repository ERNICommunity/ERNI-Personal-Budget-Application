using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Commands.Budgets;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class TransferBudgetHandler : IRequestHandler<TransferBudgetCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransferBudgetHandler(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(TransferBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(request.BudgetId, cancellationToken);
            if (budget == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget with id {request.BudgetId} not found");
            }

            if (!BudgetType.Types.Single(type => type.Id == budget.BudgetType).IsTransferable)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"Budget with id {request.BudgetId} can not be transferred");
            }

            var user = await _userRepository.GetUser(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"User with id {request.UserId} not found");
            }

            budget.UserId = request.UserId;

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
