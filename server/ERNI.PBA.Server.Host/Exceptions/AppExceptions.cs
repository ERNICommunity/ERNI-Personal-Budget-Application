using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Exceptions
{
    public static class AppExceptions
    {
        public static OperationErrorException AuthorizationException(string text = "Access forbidden")
        {
            return new OperationErrorException(StatusCodes.Status403Forbidden, text);
        }
    }
}
