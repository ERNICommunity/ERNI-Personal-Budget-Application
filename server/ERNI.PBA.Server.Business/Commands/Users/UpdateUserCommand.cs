using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Business.Utils;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class UpdateUserCommand(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserCommand> logger) : Command<UpdateUserCommand.UpdateUserModel>
    {
        private readonly ILogger _logger = logger;

        protected override async Task Execute(UpdateUserModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUser(parameter.Id, cancellationToken);

            if (user == null)
            {
                _logger.UserNotFound(parameter.Id);
                throw new OperationErrorException(ErrorCodes.UserNotFound, "Not a valid id");
            }

            user.SuperiorId = parameter.Superior;
            user.FirstName = parameter.FirstName;
            user.LastName = parameter.LastName;
            user.Username = parameter.Email;

            await unitOfWork.SaveChanges(cancellationToken);
        }

        public class UpdateUserModel
        {
            public int Id { get; set; }

            public int? Superior { get; set; }

            public string FirstName { get; set; } = null!;

            public string LastName { get; set; } = null!;

            public string Email { get; set; } = null!;
        }
    }
}
