using System;

namespace ERNI.PBA.Server.Domain.Interfaces.Export
{
    public interface IDownloadTokenManager
    {
        Guid GenerateToken(DateTime validUntil);

        bool ValidateToken(Guid token);
    }
}
