using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Budgets;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.Business.Queries.Budgets
{
    public class GetBudgetRequestsCountQuery : IGetBudgetRequestsCountQuery
    {
        private readonly IRequestRepository _requestRepository;

        public GetBudgetRequestsCountQuery(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public Task<int> ExecuteAsync(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            return _requestRepository.GetRequestsCount(parameter, cancellationToken);
        }
    }
}
