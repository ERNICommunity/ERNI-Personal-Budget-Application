using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class AddTeamRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamRequestId",
                table: "Requests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeamRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TeamRequestId",
                table: "Requests",
                column: "TeamRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_TeamRequests_TeamRequestId",
                table: "Requests",
                column: "TeamRequestId",
                principalTable: "TeamRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_TeamRequests_TeamRequestId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "TeamRequests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_TeamRequestId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TeamRequestId",
                table: "Requests");
        }
    }
}
