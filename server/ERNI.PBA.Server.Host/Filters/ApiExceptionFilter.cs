using System;
using ERNI.PBA.Server.Business.Commands.Users;
using ERNI.PBA.Server.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Host.Filters;

public sealed class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : ExceptionFilterAttribute
{
    private static readonly Action<ILogger, Exception?> _logError = LoggerMessage.Define(
        LogLevel.Error,
        new EventId(1, nameof(UpdateUserCommand)),
        "Something went wrong");

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is OperationErrorException ex)
        {
            context.HttpContext.Response.StatusCode = 400;
            context.Result = new JsonResult(ex.Message);
        }
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new JsonResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Operation failed"
            });
        }

        _logError(Logger, context.Exception);

        base.OnException(context);
    }

    public ILogger<ApiExceptionFilter> Logger { get; } = logger;
}
