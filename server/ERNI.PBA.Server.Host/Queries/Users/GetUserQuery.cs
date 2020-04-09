using ERNI.PBA.Server.Host.Model;

namespace ERNI.PBA.Server.Host.Queries.Users
{
    public class GetUserQuery : QueryBase<UserModel>
    {
        public int UserId { get; set; }
    }
}
