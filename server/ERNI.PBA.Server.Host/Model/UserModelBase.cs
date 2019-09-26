using ERNI.PBA.Server.DataAccess.Model;

namespace ERNI.PBA.Server.Host.Model
{
    public class UserModelBase
    {
        public bool IsAdmin { get; set; }

        public bool IsSuperior { get; set; }

        public bool IsViewer { get; set; }

        public UserState State { get; set; }
    }
}
