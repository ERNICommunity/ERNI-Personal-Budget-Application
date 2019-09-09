using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class BudgetTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Budgets_UserId_Year",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_UserId_Year",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "Requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Budgets",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "BudgetType",
                table: "Budgets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Budgets SET BudgetType = 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_BudgetId",
                table: "Requests",
                column: "BudgetId");

            migrationBuilder.Sql("UPDATE Requests SET BudgetId = (SELECT Id FROM Budgets WHERE Requests.UserId = Budgets.UserId AND Requests.Year = Budgets.Year)");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Budgets_BudgetId",
                table: "Requests",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Budgets_BudgetId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_BudgetId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_UserId",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "BudgetType",
                table: "Budgets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                columns: new[] { "UserId", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId_Year",
                table: "Requests",
                columns: new[] { "UserId", "Year" });

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Budgets_UserId_Year",
                table: "Requests",
                columns: new[] { "UserId", "Year" },
                principalTable: "Budgets",
                principalColumns: new[] { "UserId", "Year" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
