using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Host.Queries.Users
{
    public class GetUserQuery : QueryBase<UserModel>
    {
        public int UserId { get; set; }
    }
}
