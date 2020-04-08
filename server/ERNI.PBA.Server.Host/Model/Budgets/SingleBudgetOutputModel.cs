using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Host.Model.Budgets
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
