using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Budgets;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using MediatR;

namespace ERNI.PBA.Server.Business.Handlers.Budgets
{
    public class CreateBudgetsForAllActiveUsersHandler : IRequestHandler<CreateBudgetsForAllActiveUsersCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBudgetsForAllActiveUsersHandler(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateBudgetsForAllActiveUsersCommand request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            var budgetType = BudgetType.Types.Single(_ => _.Id == request.BudgetType);

            if (budgetType.SinglePerUser)
            {
                var budgets =
                    (await _budgetRepository.GetBudgetsByYear(DateTime.Now.Year, cancellationToken)).Where(_ =>
                        _.BudgetType == budgetType.Id).Select(_ => _.UserId).ToHashSet();
                users = users.Where(_ => !budgets.Contains(_.Id));
            }

            foreach (var user in users)
            {
                var budget = new Budget()
                {
                    UserId = user.Id,
                    Year = request.CurrentYear,
                    Amount = request.Amount,
                    BudgetType = request.BudgetType,
                    Title = request.Title
                };

                _budgetRepository.AddBudget(budget);
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
