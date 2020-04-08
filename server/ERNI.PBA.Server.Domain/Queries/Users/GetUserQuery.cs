using ERNI.PBA.Server.Domain.Output;

namespace ERNI.PBA.Server.Domain.Queries.Users
{
    public class GetUserQuery : QueryBase<UserModel>
    {
        public int UserId { get; set; }
    }
}
