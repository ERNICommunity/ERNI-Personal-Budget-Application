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
            var transactions = await _requestRepository.GetRequests(year, month, BudgetTypeEnum.PersonalBudget);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"PBA export {month}-{year}");
                worksheet.Cell("A1").Value = "Approved";
                worksheet.Cell("B1").Value = "Last name";
                worksheet.Cell("C1").Value = "First name";
                worksheet.Cell("D1").Value = "Request";
                worksheet.Cell("E1").Value = "Amount";

                var row = 1;
                foreach (var transaction in transactions)
                {
                    row++;

                    worksheet.Cell($"A{row}").Value = transaction.Request.ApprovedDate;
                    worksheet.Cell($"B{row}").Value = transaction.User.LastName;
                    worksheet.Cell($"C{row}").Value = transaction.User.FirstName;
                    worksheet.Cell($"D{row}").Value = transaction.Request.Title;
                    worksheet.Cell($"E{row}").Value = transaction.Amount;

                    for (int i = 1; i <= 3; i++)
                    {
                        worksheet.Column(i).AdjustToContents();
                    }
                }

                workbook.SaveAs(stream);
            }
        }
    }
}
