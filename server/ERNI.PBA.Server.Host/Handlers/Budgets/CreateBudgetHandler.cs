using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Commands.Budgets;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Model;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class CreateBudgetHandler : IRequestHandler<CreateBudgetCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBudgetHandler(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(request.UserId, cancellationToken);
            if (user == null || user.State != UserState.Active)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"No active user with id {request.UserId} found");
            }

            var budgets = await _budgetRepository.GetBudgets(request.UserId, request.CurrentYear, cancellationToken);
            var budgetType = BudgetType.Types.Single(_ => _.Id == request.BudgetType);
            if (budgetType.SinglePerUser && budgets.Any(b => b.BudgetType == request.BudgetType))
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest, $"User {request.UserId} already has a budget of type {budgetType.Name} assigned for this year");
            }

            var budget = new Budget
            {
                UserId = user.Id,
                Year = request.CurrentYear,
                Amount = request.Amount,
                BudgetType = request.BudgetType,
                Title = request.Title
            };

            _budgetRepository.AddBudget(budget);

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
