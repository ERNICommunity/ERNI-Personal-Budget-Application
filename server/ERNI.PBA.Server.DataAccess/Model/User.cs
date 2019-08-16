﻿namespace ERNI.PBA.Server.DataAccess.Model
{
    public class User
    {
        public int Id { get; set; }

        public string UniqueIdentifier { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public int? SuperiorId { get; set; }

        public UserState State { get; set; }

        public virtual User Superior { get; set; }
    }
}
