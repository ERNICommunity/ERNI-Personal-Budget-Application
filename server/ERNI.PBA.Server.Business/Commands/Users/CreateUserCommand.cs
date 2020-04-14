﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces;
using ERNI.PBA.Server.Domain.Interfaces.Commands.Users;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;
using ERNI.PBA.Server.Domain.Models.Payloads;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Business.Commands.Users
{
    public class CreateUserCommand : Command<CreateUserModel>, ICreateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommand(
            IUserRepository userRepository,
            IBudgetRepository budgetRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _budgetRepository = budgetRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Execute(CreateUserModel parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var userExists = await _userRepository.ExistsAsync(parameter.Email);
            if (userExists)
            {
                throw new OperationErrorException(StatusCodes.Status409Conflict);
            }

            var user = new User
            {
                FirstName = parameter.FirstName,
                LastName = parameter.LastName,
                Username = parameter.Email,
                IsAdmin = parameter.IsAdmin,
                IsSuperior = parameter.IsSuperior,
                IsViewer = parameter.IsViewer,
                SuperiorId = parameter.Superior,
                State = parameter.State
            };

            await _userRepository.AddUserAsync(user);

            if (parameter.State == UserState.Active)
            {
                var budget = new Budget
                {
                    UserId = user.Id,
                    User = user,
                    Amount = parameter.Amount,
                    Year = parameter.Year
                };
                await _budgetRepository.AddBudgetAsync(budget);
            }

            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}