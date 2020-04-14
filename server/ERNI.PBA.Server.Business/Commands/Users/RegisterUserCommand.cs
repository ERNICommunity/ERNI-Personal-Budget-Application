using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Extensions;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Domain.Models.Outputs;
using ERNI.PBA.Server.Domain.Security;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class RegisterUserCommand : Command<RegisterUserModel, UserModel>, IRegisterUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommand(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<UserModel> Execute(RegisterUserModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var username = principal.GetIdentifier(Claims.UserName);
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

            user.UniqueIdentifier = principal.GetIdentifier(Claims.UniqueIndetifier);
            user.FirstName = principal.GetIdentifier(Claims.FirstName);
            user.LastName = principal.GetIdentifier(Claims.LastName);

            await _unitOfWork.SaveChanges(cancellationToken);

            return user.ToModel();
        }
    }
}
