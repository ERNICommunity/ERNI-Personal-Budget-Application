using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Interfaces.Queries.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Responses;

namespace ERNI.PBA.Server.Business.Queries.Users
{
    public class GetUserQuery : Query<int, UserModel>, IGetUserQuery
    {
        private readonly IUserRepository _userRepository;

        public GetUserQuery(IUserRepository userRepository) => _userRepository = userRepository;

        protected override async Task<UserModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(parameter, CancellationToken.None);

            return new UserModel
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                IsSuperior = user.IsSuperior,
                IsViewer = user.IsViewer,
                FirstName = user.FirstName,
                LastName = user.LastName,
                State = user.State,
                Superior = user.Superior is not null
                    ? new SuperiorModel
                    {
                        Id = user.Superior.Id,
                        FirstName = user.Superior.FirstName,
                        LastName = user.Superior.LastName,
                    }
                    : null
            };
        }
    }
}