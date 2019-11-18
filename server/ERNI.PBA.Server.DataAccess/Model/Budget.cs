using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.DataAccess.Model
{
    public class Budget
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public BudgetType BudgetType { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }
    }
}