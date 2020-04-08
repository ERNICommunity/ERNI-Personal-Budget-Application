using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Output;
using ERNI.PBA.Server.Domain.Queries.Users;
using MediatR;

namespace ERNI.PBA.Server.Business.Handlers.Users
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserModel>
    {
        private readonly IUserRepository _userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(request.UserId, CancellationToken.None);

            return new UserModel
            {
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                IsSuperior = user.IsSuperior,
                IsViewer = user.IsViewer,
                FirstName = user.FirstName,
                LastName = user.LastName,
                State = user.State,
                Superior = user.Superior != null ? new SuperiorModel
                {
                    Id = user.Superior.Id,
                    FirstName = user.Superior.FirstName,
                    LastName = user.Superior.LastName,
                }
                    :
                    null
            };
        }
    }
}
