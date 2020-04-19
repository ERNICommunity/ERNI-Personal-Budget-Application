using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses;
using ERNI.PBA.Server.Domain.Models.Responses.PendingRequests;

namespace ERNI.PBA.Server.Business.Queries.Requests
{
    public class GetRequestsQuery : Query<GetRequestsModel, RequestModel[]>, IGetRequestsQuery
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;

        public GetRequestsQuery(
            IUserRepository userRepository,
            IRequestRepository requestRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        protected override async Task<RequestModel[]> Execute(GetRequestsModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            Expression<Func<Request, bool>> predicate;

            var userId = principal.GetId();
            var currentUser = await _userRepository.GetUser(userId, cancellationToken);
            if (currentUser.IsAdmin || currentUser.IsViewer)
            {
                predicate = request => request.Year == parameter.Year && parameter.RequestStates.Contains(request.State);
            }
            else
            {
                var subordinates = await _userRepository.GetSubordinateUsers(userId, cancellationToken);
                var subordinatesIds = subordinates.Select(u => u.Id).ToArray();
                predicate = request => request.Year == parameter.Year && parameter.RequestStates.Contains(request.State) && subordinatesIds.Contains(request.UserId);
            }

            var requests = await _requestRepository.GetRequests(predicate, cancellationToken);

            var result = requests.Select(GetModel).ToArray();

            return result;
        }

        private static RequestModel GetModel(Request request)
        {
            return new RequestModel
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Year = request.Year,
                Date = request.Date,
                CreateDate = request.CreateDate,
                State = request.State,
                User = new UserOutputModel
                {
                    Id = request.UserId,
                    FirstName = request.Budget.User.FirstName,
                    LastName = request.Budget.User.LastName
                },
                Budget = new BudgetModel
                {
                    Id = request.BudgetId,
                    Title = request.Budget.Title,
                    Type = request.Budget.BudgetType
                }
            };
        }
    }
}
