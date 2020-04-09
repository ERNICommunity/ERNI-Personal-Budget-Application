using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Queries.Budgets
{
    public class GetBudgetLeftQuery : QueryBase<UserModel[]>
    {
        public decimal Amount { get; set; }

        public int Year { get; set; }
    }
}
