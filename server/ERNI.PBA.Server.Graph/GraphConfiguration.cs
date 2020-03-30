namespace ERNI.PBA.Server.Graph
{
    public sealed class GraphConfiguration
    {
        public GraphConfiguration(string clientId, string tenantId, string clientSecret)
        {
            ClientId = clientId;
            TenantId = tenantId;
            ClientSecret = clientSecret;
        }

        public string ClientId { get; }
        public string TenantId { get; }
        public string ClientSecret { get; }
    }
}