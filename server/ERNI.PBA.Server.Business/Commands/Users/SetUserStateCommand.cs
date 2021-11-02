using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class SetUserStateCommand : Command<(int Id, UserState State)>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SetUserStateCommand(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateUserCommand> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Execute((int Id, UserState State) parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(parameter.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Not a valid id");

                throw new OperationErrorException(ErrorCodes.UserNotFound, "Not a valid id");
            }

            user.State = parameter.State;

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}