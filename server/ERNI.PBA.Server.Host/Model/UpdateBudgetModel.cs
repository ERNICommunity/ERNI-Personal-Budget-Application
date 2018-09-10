using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class UpdateBudgetModel
    {
        public int Year { get; set; }

        public User User { get; set; }

        public decimal Amount { get; set; }
    }
}
