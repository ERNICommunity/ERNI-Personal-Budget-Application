using System;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Responses
{
    public class RequestOutputModel
    {
        public int Id { get; init; }

        public string Title { get; init; }

        public decimal Amount { get; init; }

        public DateTime Date { get; init; }

        public DateTime CreateDate { get; init; }

        public RequestState State { get; init; }
    }
}
