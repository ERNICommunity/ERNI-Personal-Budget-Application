using System;

namespace ERNI.PBA.Server.Host.Commands.Requests
{
    public class UpdateRequestCommand : CommandBase<bool>
    {
        public int UserId { get; set; }

        public int RequestId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }
    }
}
