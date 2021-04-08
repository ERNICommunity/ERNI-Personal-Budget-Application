using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class Budget
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public BudgetTypeEnum BudgetType { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = null!;
    }
}