using System;

namespace ERNI.PBA.Server.Domain.Interfaces.Export
{
    public interface IDownloadTokenManager
    {
        Guid GenerateToken(DateTime validUntil, string category);

        bool ValidateToken(Guid token, string category);
    }
}
