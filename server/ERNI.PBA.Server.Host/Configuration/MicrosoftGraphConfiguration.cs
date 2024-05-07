namespace ERNI.PBA.Server.Host.Configuration;

public sealed record MicrosoftGraphConfiguration(
    string ClientId,
    string TenantId,
    string ClientSecret);
