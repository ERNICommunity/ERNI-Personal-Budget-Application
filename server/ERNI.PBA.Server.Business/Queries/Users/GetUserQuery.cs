﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERNI.PBA.Server.Business.Infrastructure;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Exceptions;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;

namespace ERNI.PBA.Server.Business.Queries.Users
{
    public class GetUserQuery(IUserRepository userRepository) : Query<int, GetUserQuery.UserModel>
    {
        protected override async Task<UserModel> Execute(int parameter, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUser(parameter, CancellationToken.None)
                       ?? throw AppExceptions.AuthorizationException();

            return new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                State = user.State,
                Email = user.Username,
                Superior = user.Superior is not null
                    ? new SuperiorModel
                    {
                        Id = user.Superior.Id,
                        FirstName = user.Superior.FirstName,
                        LastName = user.Superior.LastName,
                    }
                    : null
            };
        }

        public class UserModel
        {
            public int Id { get; init; }

            public string FirstName { get; init; } = null!;

            public string LastName { get; init; } = null!;

            public string Email { get; init; } = null!;

            public SuperiorModel? Superior { get; init; }

            public UserState State { get; init; }
        }

        public class SuperiorModel
        {
            public int Id { get; init; }

            public string FirstName { get; init; } = null!;

            public string LastName { get; init; } = null!;
        }
    }
}