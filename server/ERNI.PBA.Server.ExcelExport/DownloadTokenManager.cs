using ERNI.PBA.Server.Domain.Interfaces.Export;
using System;
using System.Collections.Generic;

namespace ERNI.PBA.Server.ExcelExport
{
    public class DownloadTokenManager : IDownloadTokenManager
    {
        private Dictionary<Guid, DateTime> _tokenDictionary = new Dictionary<Guid, DateTime>();

        public Guid GenerateToken(DateTime validUntil)
        {
            var guid = Guid.NewGuid();
            _tokenDictionary[guid] = validUntil;

            return guid;
        }

        public bool ValidateToken(Guid token) =>
            _tokenDictionary.TryGetValue(token, out var validity) && validity >= DateTime.Now;
    }
}
