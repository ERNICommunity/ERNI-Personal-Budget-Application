using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class OptionalCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_RequestCategories_CategoryId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_RequestCategories_CategoryId",
                table: "Requests",
                column: "CategoryId",
                principalTable: "RequestCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_RequestCategories_CategoryId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_RequestCategories_CategoryId",
                table: "Requests",
                column: "CategoryId",
                principalTable: "RequestCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
