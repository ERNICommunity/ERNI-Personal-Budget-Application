using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Extensions;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Output;
using ERNI.PBA.Server.Domain.Queries.Users;
using MediatR;

namespace ERNI.PBA.Server.Business.Handlers.Users
{
    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, UserModel>
    {
        private readonly IUserRepository _userRepository;

        public GetCurrentUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(request.Principal.GetId(), cancellationToken);
            if (user == null)
            {
                throw AppExceptions.AuthorizationException();
            }

            return user.ToModel();
        }
    }
}
