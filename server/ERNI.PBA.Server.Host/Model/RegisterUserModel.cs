using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Host.Model
{
    public class RegisterUserModel
    {
        public string Sub { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}
