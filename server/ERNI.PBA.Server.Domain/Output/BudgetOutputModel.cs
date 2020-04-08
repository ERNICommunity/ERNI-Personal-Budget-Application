using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Output
{
    public class BudgetOutputModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountLeft { get; set; }

        public BudgetTypeEnum Type { get; set; }

        public UserOutputModel User { get; set; }

        public IEnumerable<RequestOutputModel> Requests { get; set; }
    }
}
