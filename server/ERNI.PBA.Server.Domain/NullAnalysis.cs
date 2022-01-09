using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ERNI.PBA.Server.Domain.Exceptions;

namespace ERNI.PBA.Server.Domain
{
    public static class NullAnalysis
    {
        public static T NotNull<T>(this T? input, string errorCode, string errorDescription) where T : class =>
            input is not null ? input : throw new OperationErrorException(errorCode, errorDescription);

        public static T NotNull<T>(this T? input, string errorCode, string errorDescription) where T : struct =>
            input is not null ? input.Value : throw new OperationErrorException(errorCode, errorDescription);

        public static async Task<T> NotNullAsync<T>(this Task<T?> input, string errorCode, string errorDescription) where T : struct
        {
            var result = await input;

            return result.NotNull(errorCode, errorDescription);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2008", Justification = "ASP.NET is fine with the deafult scheduler")]
        public static Task<T> NotNullAsync<T>(this Task<T?> input, string errorCode, string errorDescription)
            where T : class =>
            input.ContinueWith(_ => _.Result.NotNull(errorCode, errorDescription));
    }
}
