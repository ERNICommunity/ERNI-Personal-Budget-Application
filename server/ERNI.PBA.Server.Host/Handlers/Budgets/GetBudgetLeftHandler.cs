using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Model;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Queries.Budgets;
using MediatR;

namespace ERNI.PBA.Server.Host.Handlers.Budgets
{
    public class GetBudgetLeftHandler : IRequestHandler<GetBudgetLeftQuery, UserModel[]>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;

        public GetBudgetLeftHandler(
            IBudgetRepository budgetRepository,
            IUserRepository userRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
        }

        public async Task<UserModel[]> Handle(GetBudgetLeftQuery request, CancellationToken cancellationToken)
        {
            var budgetAmount = (await _budgetRepository.GetTotalAmountsByYear(request.Year, cancellationToken))
                .ToDictionary(_ => _.BudgetId, _ => _.Amount);

            var budgets = await _budgetRepository.GetBudgets(request.Year, BudgetTypeEnum.PersonalBudget, cancellationToken);

            var users = (await _userRepository.GetAllUsers(cancellationToken))
                .ToDictionary(_ => _.Id);

            var usersWithBudgetLeft = new List<UserModel>();

            foreach (var budget in budgets)
            {
                if (budgetAmount[budget.Id] + request.Amount <= budget.Amount)
                {
                    var user = users[budget.UserId];

                    usersWithBudgetLeft.Add(new UserModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        IsAdmin = user.IsAdmin,
                        IsSuperior = user.IsSuperior,
                        IsViewer = user.IsViewer,
                        LastName = user.LastName,
                        State = user.State,
                        Superior = user.Superior != null ? new SuperiorModel
                        {
                            FirstName = user.Superior.FirstName,
                            Id = user.Superior.Id,
                            LastName = user.Superior.LastName
                        }
                            :
                            null
                    });
                }
            }

            return usersWithBudgetLeft.ToArray();
        }
    }
}
