using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class UpdateUserCommand : Command<UpdateUserModel>, IUpdateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UpdateUserCommand(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateUserCommand> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Execute(UpdateUserModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(parameter.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Not a valid id");

                throw new OperationErrorException(StatusCodes.Status404NotFound, "Not a valid id");
            }

            user.IsAdmin = parameter.IsAdmin;
            user.IsViewer = parameter.IsViewer;
            user.IsSuperior = parameter.IsSuperior;
            user.SuperiorId = parameter.Superior?.Id;
            user.State = parameter.State;

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
