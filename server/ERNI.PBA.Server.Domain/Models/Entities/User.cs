using System;
using ERNI.PBA.Server.Domain.Enums;

namespace ERNI.PBA.Server.Domain.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string? UniqueIdentifier { get; set; } = null!;

        public Guid ObjectId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public UserState State { get; set; }

        public int Utilization { get; set; }

        public int? SuperiorId { get; set; }

        public User? Superior { get; set; } = null!;
    }
}