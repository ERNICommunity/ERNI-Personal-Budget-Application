using System;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses.PendingRequests
{
    public class RequestModel
    {
        public int Id { get; init; }

        public DateTime Date { get; init; }

        public DateTime CreateDate { get; init; }

        public string Title { get; init; } = null!;

        public decimal Amount { get; init; }

        public RequestState State { get; init; }

        public UserOutputModel User { get; init; } = null!;

        public int Year { get; init; }
    }
}