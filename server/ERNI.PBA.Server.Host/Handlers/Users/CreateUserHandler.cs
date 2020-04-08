using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models;
using ERNI.PBA.Server.Host.Commands.Users;
using ERNI.PBA.Server.Host.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Handlers.Users
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserHandler(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _userRepository.ExistsAsync(request.Email);
            if (userExists)
            {
                throw new OperationErrorException(StatusCodes.Status409Conflict);
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Email,
                IsAdmin = request.IsAdmin,
                IsSuperior = request.IsSuperior,
                IsViewer = request.IsViewer,
                SuperiorId = request.Superior,
                State = request.State
            };

            await _userRepository.AddUserAsync(user);

            if (request.State == UserState.Active)
            {
                var budget = new Budget
                {
                    UserId = user.Id,
                    User = user,
                    Amount = request.Amount,
                    Year = request.Year
                };
                await _budgetRepository.AddBudgetAsync(budget);
            }

            await _unitOfWork.SaveChanges(cancellationToken);

            return true;
        }
    }
}
