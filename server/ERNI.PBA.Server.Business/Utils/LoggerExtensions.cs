using System;
using ERNI.PBA.Server.Business.Commands.Users;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Business.Utils
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, int, Exception?> _requestNotFound = LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(1, nameof(UpdateUserCommand)),
            "Request with id {UserId} does not exist");

        private static readonly Action<ILogger, int, Exception?> _userNotFound = LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(1, nameof(UpdateUserCommand)),
            "User with id {UserId} does not exist");

        public static void RequestNotFound(this ILogger logger, int requestId) => _requestNotFound(logger, requestId, null);

        public static void UserNotFound(this ILogger logger, int userId) => _userNotFound(logger, userId, null);

    }
}