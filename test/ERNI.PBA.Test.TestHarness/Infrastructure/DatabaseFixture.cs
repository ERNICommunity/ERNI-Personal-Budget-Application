using System;
using System.Diagnostics;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace ERNI.PBA.Test.TestHarness.Infrastructure
{
    public class DatabaseFixture : IDisposable
    {
        private IHost _host;
        private ClientFacade _client;

        public DatabaseFixture()
        {
            var time = DateTime.Now.ToString("yyMMddHHmm");
            var guid = Guid.NewGuid();
            var id = guid.ToString().Substring(0, 8);

            DatabaseName = $"PbaIntTest_{time}_{id}";

            Debug.WriteLine(
                $"Initializing test fixture. Database name: {DatabaseName}.");
        }

        public string DatabaseName { get; }

        public Action DisposeAction { get; set; }

        public IHost GetHost()
        {
            return _host ?? (_host = Server.StartBackendServer(databasename: DatabaseName));
        }

        public ClientFacade StartOrGetClient()
        {
            if (_client == null)
            {
                _client = new ClientFacade(GetHost().GetTestServer());
            }

            return _client;
        }

        public void Dispose()
        {
            if (DisposeAction != null)
            {
                var action = DisposeAction;
                DisposeAction = null;
                action();
            }

            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
