using System;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class UpdateRequestModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public string Url { get; set; }
    }
}
