using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Business.Queries.TeamBudgets
{
    public class CreateTeamRequestCommand(
        IUserRepository userRepository,
        ITeamBudgetFacade teamBudgetFacade,
        IRequestRepository requestRepository,
        IUnitOfWork unitOfWork) : Command<CreateTeamRequestCommand.NewTeamRequestModel, int>
    {
        protected override async Task<int> Execute(NewTeamRequestModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userId = principal.GetId();
            var user = await userRepository.GetUser(userId, cancellationToken)
                       ?? throw AppExceptions.AuthorizationException();

            var currentYear = DateTime.Now.Year;

            if (parameter.Employees.Distinct().Count() != parameter.Employees.Length)
            {
                throw new OperationErrorException(ErrorCodes.ValidationError, "User list has to be unique.");
            }

            var allBudgets =
                await teamBudgetFacade.GetTeamBudgets(user.Id, DateTime.Now.Year, cancellationToken);
            var dict = allBudgets.ToDictionary(_ => _.Employee.Id);
            var unknownUsers = parameter.Employees.Where(id => !dict.ContainsKey(id)).ToList();

            if (unknownUsers.Count != 0)
            {
                throw new OperationErrorException(ErrorCodes.ValidationError, $"Employees not found: {string.Join(",", unknownUsers)}");
            }

            var teamBudgets = parameter.Employees.Select(_ => dict[_])
                .Select(_ => new TeamBudget()
                {
                    BudgetId = _.BudgetId,
                    Amount = _.TotalAmount - _.SpentAmount,
                    UserId = _.Employee.Id
                });

            if (parameter.Amount <= 0.0m)
            {
                throw new OperationErrorException(ErrorCodes.InvalidAmount, $"The requested amount ({parameter.Amount}) has to be positive.");
            }

            var availableFunds = teamBudgets.Sum(_ => _.Amount);
            if (availableFunds < parameter.Amount)
            {
                throw new OperationErrorException(ErrorCodes.InvalidAmount, $"The requested amount {parameter.Amount} exceeds the limit.");
            }

            var transactions = TransactionCalculator.Create(teamBudgets, parameter.Amount);
            var request = new Request
            {
                UserId = user.Id,
                Year = currentYear,
                Title = parameter.Title,
                Amount = parameter.Amount,
                CreateDate = DateTime.Now,
                State = RequestState.Approved,
                RequestType = BudgetTypeEnum.TeamBudget,
                Transactions = transactions
            };

            await requestRepository.AddRequest(request);

            await unitOfWork.SaveChanges(cancellationToken);

            return request.Id;
        }

        public class NewTeamRequestModel
        {
            public string Title { get; set; } = null!;

            public decimal Amount { get; set; }

            public int[] Employees { get; set; } = null!;
        }
    }
}
