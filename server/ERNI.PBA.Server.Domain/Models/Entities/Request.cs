using System;
using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class Request
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int? CategoryId { get; set; }

        public int BudgetId { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
        public string Url { get; set; } = null!;
#pragma warning restore CA1056 // URI-like properties should not be strings

        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public RequestState State { get; set; }

        public Budget Budget { get; set; } = null!;

        public RequestCategory Category { get; set; } = null!;

        public User User { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = null!;

        public override string ToString() =>
            $"{Title} ({Amount}) by {Budget.User.FirstName} {Budget.User.LastName}";
    }
}