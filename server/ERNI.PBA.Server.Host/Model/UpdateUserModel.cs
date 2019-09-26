using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class UpdateUserModel : UserModelBase
    {
        public int Id { get; set; }

        public User Superior { get; set; }
    }
}