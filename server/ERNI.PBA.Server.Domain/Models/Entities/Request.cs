using System;
using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.API;

namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class Request
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int? CategoryId { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
        public string? Url { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public decimal? InvoicedAmount { get; set; }

        public DateTime? Date { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        public RequestState State { get; set; }

        public RequestCategory? Category { get; set; }

        public BudgetTypeEnum RequestType { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = null!;
    }
}