using ERNI.PBA.Server.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ERNI.PBA.Server.Host.Filters
{
    public sealed class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) => Logger = logger;

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationErrorException ex)
            {
                context.HttpContext.Response.StatusCode = ex.HttpStatusCode;
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

                Logger.LogError($"Something went wrong: {context.Exception}");
            }

            base.OnException(context);
        }

        public ILogger<ApiExceptionFilter> Logger { get; }
    }
}
