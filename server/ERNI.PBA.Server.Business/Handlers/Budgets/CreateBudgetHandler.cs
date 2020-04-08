using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Budgets;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Handlers.Budgets
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
