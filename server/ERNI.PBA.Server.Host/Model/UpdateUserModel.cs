using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class UpdateUserModel
    {
        public int Id { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public User Superior { get; set; }

        public UserState State { get; set; } 
    }
}