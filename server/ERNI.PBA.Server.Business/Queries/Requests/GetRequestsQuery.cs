using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Requests;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Requests
{
    public class GetRequestsQuery : Query<GetRequestsModel, IGetRequestsQuery.RequestModel[]>, IGetRequestsQuery
    {
        private readonly IRequestRepository _requestRepository;

        public GetRequestsQuery(IRequestRepository requestRepository) => _requestRepository = requestRepository;

        protected override async Task<IGetRequestsQuery.RequestModel[]> Execute(GetRequestsModel parameter, ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {

            var requests = await _requestRepository.GetRequests(
                request => request.Year == parameter.Year && parameter.RequestStates.Contains(request.State),
                cancellationToken);

            var result = requests.Select(GetModel).ToArray();

            return result;
        }

        private static IGetRequestsQuery.RequestModel GetModel(Request request)
        {
            var t = request.Transactions.First();

            return new()
            {
                Id = request.Id,
                Title = request.Title,
                Amount = request.Amount,
                Year = request.Year,
                Date = request.Date,
                CreateDate = request.CreateDate,
                InvoicedAmount = request.InvoicedAmount,
                State = request.State,
                User = new UserOutputModel
                {
                    Id = request.UserId, FirstName = request.User.FirstName, LastName = request.User.LastName
                },
                Budget = new IGetRequestsQuery.RequestModel.BudgetModel
                {
                    Id = t.BudgetId,
                    Title = t.Budget.Title,
                    Type = BudgetType.Types.Single(_ => _.Id == t.Budget.BudgetType)
                }
            };
        }
    }
}
