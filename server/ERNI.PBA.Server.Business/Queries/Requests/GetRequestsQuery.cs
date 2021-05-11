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
        private readonly IInvoiceImageRepository _invoiceRepository;

        public GetRequestsQuery(IRequestRepository requestRepository, IInvoiceImageRepository invoiceRepository) =>
            (_requestRepository, _invoiceRepository) = (requestRepository, invoiceRepository);

        protected override async Task<IGetRequestsQuery.RequestModel[]> Execute(GetRequestsModel parameter, ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {

            var requests = await _requestRepository.GetRequests(
                request => request.Year == parameter.Year && parameter.RequestStates.Contains(request.State),
                cancellationToken);

            var invoiceCounts =
                (await _invoiceRepository.GetInvoiceCounts(requests.Select(_ => _.Id).ToArray(), cancellationToken))
                .ToDictionary(_ => _.requestId, _ => _.invoiceCount);

            var result = requests.Select(request =>
                GetModel(request, invoiceCounts.TryGetValue(request.Id, out var count) ? count : 0)).ToArray();

            return result;
        }

        private static IGetRequestsQuery.RequestModel GetModel(Request request, int invoiceCount)
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
                InvoiceCount = invoiceCount,
                State = request.State,
                User = new UserOutputModel
                {
                    Id = request.UserId,
                    FirstName = request.User.FirstName,
                    LastName = request.User.LastName
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
