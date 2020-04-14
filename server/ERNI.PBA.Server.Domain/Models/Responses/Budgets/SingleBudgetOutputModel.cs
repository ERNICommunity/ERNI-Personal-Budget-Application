using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Domain.Models.Responses.Budgets
{
    public class SingleBudgetOutputModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public decimal Amount { get; set; }

        public BudgetTypeEnum Type { get; set; }

        public User User { get; set; }
    }
}
