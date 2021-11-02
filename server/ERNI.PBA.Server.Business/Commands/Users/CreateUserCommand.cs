using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class CreateUserCommand : Command<CreateUserCommand.CreateUserModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommand(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(CreateUserModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userExists = await _userRepository.ExistsAsync(parameter.Email);
            if (userExists)
            {
                throw new OperationErrorException(ErrorCodes.UnknownError, $"User with email '{parameter.Email}' already exists");
            }

            if (string.IsNullOrWhiteSpace(parameter.FirstName) || string.IsNullOrWhiteSpace(parameter.LastName))
            {
                throw new OperationErrorException(ErrorCodes.ValidationError, "Invalid name");
            }

            var user = new User
            {
                FirstName = parameter.FirstName.Trim(),
                LastName = parameter.LastName.Trim(),
                Username = parameter.Email,
                SuperiorId = parameter.Superior,
                State = parameter.State
            };

            await _userRepository.AddUserAsync(user);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public class CreateUserModel
        {
            public string FirstName { get; set; } = null!;

            public string LastName { get; set; } = null!;

            public string Email { get; set; } = null!;

            public int Year { get; set; }

            public int? Superior { get; set; }

            public UserState State { get; set; }
        }
    }
}