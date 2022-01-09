using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Models.Responses.Budgets
{
    public class SingleBudgetOutputModel
    {
        public int Id { get; init; }

        public string Title { get; init; } = null!;

        public int Year { get; init; }

        public decimal Amount { get; init; }

        public BudgetTypeEnum Type { get; init; }

        public User User { get; init; } = null!;
    }
}