using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class SetUserStateCommand(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserCommand> logger) : Command<(int Id, UserState State)>
    {
        private readonly ILogger _logger = logger;

        protected override async Task Execute((int Id, UserState State) parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUser(parameter.Id, cancellationToken);

            if (user == null)
            {
                _logger.UserNotFound(parameter.Id);

                throw new OperationErrorException(ErrorCodes.UserNotFound, "Not a valid id");
            }

            user.State = parameter.State;

            await unitOfWork.SaveChanges(cancellationToken);
        }
    }
}