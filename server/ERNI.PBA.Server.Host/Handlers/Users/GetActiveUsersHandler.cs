using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Output;
using ERNI.PBA.Server.Host.Queries.Users;
using MediatR;

namespace ERNI.PBA.Server.Host.Handlers.Users
{
    public class GetActiveUsersHandler : IRequestHandler<GetActiveUsersQuery, IEnumerable<UserModel>>
    {
        private readonly IUserRepository _userRepository;

        public GetActiveUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserModel>> Handle(GetActiveUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsers(_ => _.State == UserState.Active, cancellationToken);

            return users.Select(_ => new UserModel
            {
                Id = _.Id,
                FirstName = _.FirstName,
                LastName = _.LastName,
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);
        }
    }
}
