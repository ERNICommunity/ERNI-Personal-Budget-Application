using ClosedXML.Excel;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ERNI.Rmt.ExcelExport
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly IRequestRepository _requestRepository;

        public ExcelExportService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task Export(Stream stream, int year, int month, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetRequests(year, month, BudgetTypeEnum.PersonalBudget);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"PBA export {month}-{year}");
                worksheet.Cell("A1").Value = "Last name";
                worksheet.Cell("B1").Value = "First name";
                worksheet.Cell("C1").Value = "Request";
                worksheet.Cell("D1").Value = "Amount";

                var row = 1;
                foreach (var request in requests)
                {
                    row++;

                    worksheet.Cell($"A{row}").Value = request.User.LastName;
                    worksheet.Cell($"B{row}").Value = request.User.FirstName;
                    worksheet.Cell($"C{row}").Value = request.Title;
                    worksheet.Cell($"D{row}").Value = request.Amount;

                    for (int i = 1; i <= 3; i++)
                    {
                        worksheet.Column(i).AdjustToContents();
                    }

                    workbook.SaveAs(stream);
                }
            }
        }
    }
}
