﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Domain.Interfaces.Infrastructure
{
    public interface ICommand<in T, TResult>
    {
        Task<TResult> ExecuteAsync(T parameter, ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}
