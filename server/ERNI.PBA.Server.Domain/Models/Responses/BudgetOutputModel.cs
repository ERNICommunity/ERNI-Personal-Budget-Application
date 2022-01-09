using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class BudgetOutputModel
    {
        public int Id { get; init; }

        public string Title { get; init; } = null!;

        public int Year { get; init; }

        public decimal Amount { get; init; }

        public decimal AmountLeft { get; init; }

        public BudgetTypeEnum Type { get; init; }

        public UserOutputModel User { get; init; } = null!;

        public IEnumerable<RequestOutputModel> Requests { get; init; } = null!;
    }
}