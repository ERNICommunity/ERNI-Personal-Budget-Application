using ERNI.PBA.Server.Domain.Models.Outputs;

namespace ERNI.PBA.Server.Domain.Queries.Users
{
    public class GetUserQuery : QueryBase<UserModel>
    {
        public int UserId { get; set; }
    }
}
