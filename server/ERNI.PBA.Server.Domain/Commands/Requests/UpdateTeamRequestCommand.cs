using System;
using MediatR;

namespace ERNI.PBA.Server.Domain.Commands.Requests
{
    public class UpdateTeamRequestCommand : IRequest<bool>
    {
        public int UserId { get; set; }

        public int RequestId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }
    }
}
