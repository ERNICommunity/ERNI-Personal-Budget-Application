using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Queries
{
    public class GetBudgetByYearQuery : QueryBase<BudgetOutputModel[]>
    {
        public int UserId { get; set; }

        public int Year { get; set; }
    }
}
