using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Host.Handlers.Users
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UpdateUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateUserHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(request.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Not a valid id");

                throw new OperationErrorException(StatusCodes.Status404NotFound, "Not a valid id");
            }

            user.IsAdmin = request.IsAdmin;
            user.IsViewer = request.IsViewer;
            user.IsSuperior = request.IsSuperior;
            user.SuperiorId = request.Superior?.Id;
            user.State = request.State;

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
