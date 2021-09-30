using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class IntroduceRequestType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Budgets_BudgetId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Requests_RequestId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BudgetId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RequestId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE Transactions SET RequestType = (SELECT BudgetType FROM Budgets WHERE Budgets.Id = Transactions.BudgetId)");

            migrationBuilder.Sql(
                "UPDATE Requests SET RequestType = (SELECT TOP 1 Transactions.RequestType FROM Transactions WHERE Requests.Id = Transactions.RequestId)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Requests_Id_RequestType",
                table: "Requests",
                columns: new[] { "Id", "RequestType" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Budgets_Id_BudgetType",
                table: "Budgets",
                columns: new[] { "Id", "BudgetType" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BudgetId_RequestType",
                table: "Transactions",
                columns: new[] { "BudgetId", "RequestType" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RequestId_RequestType",
                table: "Transactions",
                columns: new[] { "RequestId", "RequestType" });

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Budgets_BudgetId_RequestType",
                table: "Transactions",
                columns: new[] { "BudgetId", "RequestType" },
                principalTable: "Budgets",
                principalColumns: new[] { "Id", "BudgetType" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Requests_RequestId_RequestType",
                table: "Transactions",
                columns: new[] { "RequestId", "RequestType" },
                principalTable: "Requests",
                principalColumns: new[] { "Id", "RequestType" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Budgets_BudgetId_RequestType",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Requests_RequestId_RequestType",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BudgetId_RequestType",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RequestId_RequestType",
                table: "Transactions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Requests_Id_RequestType",
                table: "Requests");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Budgets_Id_BudgetType",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Requests");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BudgetId",
                table: "Transactions",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RequestId",
                table: "Transactions",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Budgets_BudgetId",
                table: "Transactions",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Requests_RequestId",
                table: "Transactions",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
