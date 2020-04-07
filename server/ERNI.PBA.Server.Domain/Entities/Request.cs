using System;
using System.Collections.Generic;

namespace ERNI.PBA.Server.Domain.Entities
{
    public class Request
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int? CategoryId { get; set; }

        public int BudgetId { get; set; }

        public string Url { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public DateTime CreateDate { get; set; }

        public RequestState State { get; set; }

        public Budget Budget { get; set; }

        public RequestCategory Category { get; set; }

        public User User { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public override string ToString()
        {
            return $"{Title} ({Amount}) by {Budget?.User?.FirstName} {Budget?.User?.LastName}";
        }
    }
}