using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses.PendingRequests
{
    public class BudgetModel
    {
        public int Id { get; init; }

        public BudgetTypeEnum Type { get; init; }

        public string Title { get; init; } = null!;
    }
}