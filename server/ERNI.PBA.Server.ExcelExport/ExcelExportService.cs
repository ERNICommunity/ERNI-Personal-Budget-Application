using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ERNI.PBA.Server.Domain.Enums;
using ERNI.PBA.Server.Domain.Interfaces.Export;
using ERNI.PBA.Server.Domain.Interfaces.Repositories;
using ERNI.PBA.Server.Domain.Models.Entities;

namespace ERNI.Rmt.ExcelExport
{
    public class ExcelExportService(IRequestRepository requestRepository) : IExcelExportService
    {
        public async Task Export(Stream stream, int year, int month, CancellationToken cancellationToken)
        {
            var transactions = (await requestRepository.GetRequests(year, month, BudgetTypeEnum.PersonalBudget))
                .ToLookup(_ => _.Request.State);

            using var workbook = new XLWorkbook();

            AddSheet(workbook, $"Approved {month}-{year}", transactions[RequestState.Approved]);
            // AddSheet(workbook, $"Completed {month}-{year}", transactions[RequestState.Completed]);

            workbook.SaveAs(stream);
        }

        private static void AddSheet(XLWorkbook workbook, string title, IEnumerable<Transaction> transactions)
        {
            var worksheet = workbook.Worksheets.Add(title);
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
                worksheet.Cell($"B{row}").Value = transaction.Request.User.LastName;
                worksheet.Cell($"C{row}").Value = transaction.Request.User.FirstName;
                worksheet.Cell($"D{row}").Value = transaction.Request.Title;
                worksheet.Cell($"E{row}").Value = transaction.Amount;

                for (var i = 3; i >= 1; i--)
                {
                    worksheet.Column(i).AdjustToContents();
                }
            }
        }
    }
}