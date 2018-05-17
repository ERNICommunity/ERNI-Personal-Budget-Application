using System;
using System.Collections.Generic;
using System.Text;

namespace ERNI.PBA.Server.DataAccess.Model
{
    public class User
    {
        public int Id { get; set; }

        public string UniqueIdentifier { get; set; }

        public bool IsAdmin { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public User Superior { get; set; }

        public int? SuperiorId { get; set; }

        public bool IsActive { get; set; }
    }
}
