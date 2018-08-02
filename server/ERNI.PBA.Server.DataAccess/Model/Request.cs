using System;

namespace ERNI.PBA.Server.DataAccess.Model
{
    public class Request
    {
        public int Id { get; set; }

        public Budget Budget { get; set; }

        public int Year { get; set; }

        public int CategoryId { get; set; }

        public RequestCategory Category { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public RequestState State { get; set; }
    }
}