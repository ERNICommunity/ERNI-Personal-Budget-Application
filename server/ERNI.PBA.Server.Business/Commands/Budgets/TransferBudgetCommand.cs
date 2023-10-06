using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Payloads;

namespace ERNI.PBA.Server.Business.Commands.Budgets
{
    public class TransferBudgetCommand : Command<TransferBudgetModel>, ITransferBudgetCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransferBudgetCommand(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(TransferBudgetModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudget(parameter.BudgetId, cancellationToken)
                ?? throw new OperationErrorException(ErrorCodes.BudgetNotFound, $"Budget with id {parameter.BudgetId} not found");

            if (!BudgetType.Types.Single(type => type.Id == budget.BudgetType).IsTransferable)
            {
                throw new OperationErrorException(ErrorCodes.UnknownError, $"Budget with id {parameter.BudgetId} can not be transferred");
            }

            if (await _userRepository.GetUser(parameter.UserId, cancellationToken) == null)
            {
                throw new OperationErrorException(ErrorCodes.UserNotFound, $"User with id {parameter.UserId} not found");
            }

            budget.UserId = parameter.UserId;

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
