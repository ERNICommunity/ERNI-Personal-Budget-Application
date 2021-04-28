using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Domain.Exceptions
{
    public static class AppExceptions
    {
        public static OperationErrorException AuthorizationException(string text = "Access forbidden") =>
            new(ErrorCodes.AccessDenied, text);
    }
}