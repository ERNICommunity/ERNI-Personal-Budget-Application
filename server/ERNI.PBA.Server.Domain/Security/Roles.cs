namespace ERNI.PBA.Server.Domain.Security
{
    public static class Roles
    {
        public const string Employee = "PBA.Employee";

        public const string Admin = "PBA.Admin";

        public const string Finance = "PBA.Finance";

        public const string AnyRole = "PBA.Employee, PBA.Admin, PBA.Finance";
    }
}