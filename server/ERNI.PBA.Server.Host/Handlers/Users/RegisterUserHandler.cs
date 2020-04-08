using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Output;
using ERNI.PBA.Server.Host.Exceptions;
using ERNI.PBA.Server.Host.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Users
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var username = request.Principal.GetIdentifier(Claims.UserName);
            if (string.IsNullOrWhiteSpace(username))
            {
                throw AppExceptions.AuthorizationException();
            }

            var user = await _userRepository.GetAsync(username);
            if (user == null)
            {
                throw AppExceptions.AuthorizationException();
            }

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationErrorException(StatusCodes.Status400BadRequest);
            }

            user.UniqueIdentifier = request.Principal.GetIdentifier(Claims.UniqueIndetifier);
            user.FirstName = request.Principal.GetIdentifier(Claims.FirstName);
            user.LastName = request.Principal.GetIdentifier(Claims.LastName);

            await _unitOfWork.SaveChanges(cancellationToken);

            return user.ToModel();
        }
    }
}
