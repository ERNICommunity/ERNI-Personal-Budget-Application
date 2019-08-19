using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class BudgetLeftModel
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public int RequestId { get; set; }
    }
}