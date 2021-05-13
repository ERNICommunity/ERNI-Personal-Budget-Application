namespace ERNI.PBA.Server.Domain.Exceptions
{
    public static class ErrorCodes
    {
        public static string AttachmentDataNotFound => nameof(AttachmentDataNotFound);

        public static string InvalidId => nameof(InvalidId);

        public static string AccessDenied => nameof(AccessDenied);

        public static string UserNotFound => nameof(UserNotFound);

        public static string BudgetNotFound => nameof(BudgetNotFound);

        public static string InvalidAmount => nameof(InvalidAmount);
        public static string InvalidTitle => nameof(InvalidTitle);
        public static string RequestNotFound => nameof(RequestNotFound);
        public static string ValidationError => nameof(ValidationError);

        public static string RequestNotApproved => nameof(RequestNotApproved);

        public static string UnknownError => nameof(UnknownError);
    }
}
