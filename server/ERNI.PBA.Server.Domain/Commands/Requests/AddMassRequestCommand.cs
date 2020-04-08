using System;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Models;

namespace ERNI.PBA.Server.Domain.Commands.Requests
{
    public class AddMassRequestCommand : CommandBase<bool>
    {
        public int UserId { get; set; }

        public string Title { get; set; }

        public decimal Amount { get; set; }

        public RequestState State { get; set; }

        public int CurrentYear { get; set; }

        public DateTime Date { get; set; }

        public User[] Users { get; set; }
    }
}
