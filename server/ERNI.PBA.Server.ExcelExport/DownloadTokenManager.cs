using System;
using System.Collections.Generic;
using ERNI.PBA.Server.Domain.Interfaces.Export;

namespace ERNI.Rmt.ExcelExport
{
    public class DownloadTokenManager : IDownloadTokenManager
    {
        private readonly Dictionary<Guid, TokenInfo> _tokenDictionary = new();

        public Guid GenerateToken(DateTime validUntil, string category)
        {
            var guid = Guid.NewGuid();
            _tokenDictionary[guid] = new TokenInfo(validUntil, category);

            return guid;
        }

        public bool ValidateToken(Guid token, string category) =>
            _tokenDictionary.TryGetValue(token, out var tokenInfo) && tokenInfo.ValidUntil >= DateTime.Now &&
            tokenInfo.Category == category;

        public record TokenInfo(DateTime ValidUntil, string Category);
    }
}
