using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Queries.Budgets
{
    public class GetBudgetLeftQuery : QueryBase<UserModel[]>
    {
        public decimal Amount { get; set; }

        public int Year { get; set; }
    }
}
