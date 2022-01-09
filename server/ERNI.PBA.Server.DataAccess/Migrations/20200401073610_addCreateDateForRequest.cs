using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class addCreateDateForRequest : Migration
#pragma warning restore IDE1006 // Naming Styles
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Requests",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Requests");
        }
    }
}
