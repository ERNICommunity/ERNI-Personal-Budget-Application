using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.PBA.Server.Domain.Interfaces.Export
{
    public interface IExcelExportService
    {
        Task Export(Stream stream, int year, int month, CancellationToken cancellationToken);
    }
}
