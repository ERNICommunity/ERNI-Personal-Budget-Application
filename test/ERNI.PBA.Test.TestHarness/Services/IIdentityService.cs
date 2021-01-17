using ERNI.PBA.Test.TestHarness.Infrastructure;

namespace ERNI.PBA.Test.TestHarness.Services
{
    public interface IIdentityService
    {
        AzureIdentity AzureIdentity { get; }

        void SetIdentity(AzureIdentity azureIdentity);
    }
}
