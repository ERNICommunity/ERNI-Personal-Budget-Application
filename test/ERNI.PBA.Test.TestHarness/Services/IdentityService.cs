using ERNI.PBA.Test.TestHarness.Infrastructure;

namespace ERNI.PBA.Test.TestHarness.Services
{
    internal class IdentityService : IIdentityService
    {
        public AzureIdentity AzureIdentity { get; private set; } = AzureIdentity.Admin;

        public void SetIdentity(AzureIdentity azureIdentity)
        {
            AzureIdentity = azureIdentity;
        }
    }
}
