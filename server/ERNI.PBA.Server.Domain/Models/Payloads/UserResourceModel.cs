using System;

namespace ERNI.PBA.Server.Domain.Models.Payloads
{
    public class UserResourceModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public Guid? ManagerId { get; set; }
        public int Fte { get; set; }
    }
}