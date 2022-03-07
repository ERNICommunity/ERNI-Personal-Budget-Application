using System;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Infrastructure;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Domain.Interfaces.Queries.Requests
{
    public interface IGetRequestsQuery : IQuery<GetRequestsModel, IGetRequestsQuery.RequestModel[]>
    {
        public class RequestModel
        {
            public class BudgetModel
            {
                public int Id { get; init; }

                public string Title { get; init; } = null!;

                public BudgetType Type { get; init; } = null!;
            }

            public int Id { get; init; }

            public DateTime CreateDate { get; init; }

            public BudgetModel Budget { get; init; } = null!;

            public string Title { get; init; } = null!;

            public decimal Amount { get; init; }

            public decimal? InvoicedAmount { get; set; }

            public int InvoiceCount { get; init; }

            public RequestState State { get; init; }

            public UserOutputModel User { get; init; } = null!;

            public int Year { get; init; }
        }
    }
}
