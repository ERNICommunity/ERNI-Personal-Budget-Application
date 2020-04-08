using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public BudgetTypeEnum BudgetType { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public ICollection<Request> Requests { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}