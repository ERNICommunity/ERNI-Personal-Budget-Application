namespace ERNI.PBA.Server.Domain.Security
{
    public static class Claims
    {
        public const string FirstName = "given_name";
        public const string LastName = "family_name";
        public const string UserName = "upn";
        public const string UniqueIndetifier = "sub";
        public const string Role = "Role";
        public const string Id = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    }
}