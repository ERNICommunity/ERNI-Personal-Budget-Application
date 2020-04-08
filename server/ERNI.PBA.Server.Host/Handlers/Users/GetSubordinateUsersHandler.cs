using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Host.Model;
using ERNI.PBA.Server.Host.Queries.Users;
using ERNI.PBA.Server.Host.Utils;
using MediatR;

namespace ERNI.PBA.Server.Host.Handlers.Users
{
    public class GetSubordinateUsersHandler : IRequestHandler<GetSubordinateUsersQuery, IEnumerable<UserModel>>
    {
        private readonly IUserRepository _userRepository;

        public GetSubordinateUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserModel>> Handle(GetSubordinateUsersQuery request, CancellationToken cancellationToken)
        {
            User[] users;

            var user = await _userRepository.GetUser(request.Principal.GetId(), cancellationToken);
            if (user.IsAdmin)
            {
                users = await _userRepository.GetAllUsers(cancellationToken);
            }
            else
            {
                users = await _userRepository.GetSubordinateUsers(user.Id, cancellationToken);
            }

            return users.Select(_ => new UserModel
            {
                Id = _.Id,
                IsAdmin = _.IsAdmin,
                IsSuperior = _.IsSuperior,
                IsViewer = _.IsViewer,
                FirstName = _.FirstName,
                LastName = _.LastName,
                State = _.State,
                Superior = _.Superior != null ? new SuperiorModel
                {
                    Id = _.Superior.Id,
                    FirstName = _.Superior.FirstName,
                    LastName = _.Superior.LastName,
                }
                    :
                    null
            }).OrderBy(_ => _.LastName).ThenBy(_ => _.FirstName);
        }
    }
}
