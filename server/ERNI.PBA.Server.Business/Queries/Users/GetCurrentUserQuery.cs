using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Extensions;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Users
{
    public class GetCurrentUserQuery : Query<UserModel>, IGetCurrentUserQuery
    {
        private readonly IUserRepository _userRepository;

        public GetCurrentUserQuery(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task<UserModel> Execute(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(principal.GetId(), cancellationToken);
            if (user == null)
            {
                throw AppExceptions.AuthorizationException();
            }

            return user.ToModel();
        }
    }
}
