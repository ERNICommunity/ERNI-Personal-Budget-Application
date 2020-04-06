using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.DataAccess.Model;
using ERNI.PBA.Server.DataAccess.Repository;
using ERNI.PBA.Server.Host.Model.PendingRequests;
using ERNI.PBA.Server.Host.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ERNI.PBA.Server.Host.Handlers.Requests
{
    public class GetRequestsHandler : IRequestHandler<GetRequestsQuery, RequestModel[]>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;

        public GetRequestsHandler(
            IUserRepository userRepository,
            IRequestRepository requestRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        public async Task<RequestModel[]> Handle(GetRequestsQuery query, CancellationToken cancellationToken)
        {
            Expression<Func<Request, bool>> predicate;

            var currentUser = await _userRepository.GetUser(query.UserId, cancellationToken);
            if (currentUser.IsAdmin || currentUser.IsViewer)
            {
                predicate = request => request.Year == query.Year && query.RequestStates.Contains(request.State);
            }
            else
            {
                var subordinates = await _userRepository.GetSubordinateUsers(query.UserId, cancellationToken);
                var subordinatesIds = subordinates.Select(u => u.Id).ToArray();
                predicate = request => request.Year == query.Year && query.RequestStates.Contains(request.State) && subordinatesIds.Contains(request.UserId);
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
