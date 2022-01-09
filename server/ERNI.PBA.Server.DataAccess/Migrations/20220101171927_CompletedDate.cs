using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class CompletedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "Requests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "Requests");
        }
    }
}
