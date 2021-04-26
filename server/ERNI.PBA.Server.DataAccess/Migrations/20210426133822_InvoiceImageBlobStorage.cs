using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class InvoiceImageBlobStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "InvoiceImage");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "InvoiceImage",
                newName: "MimeType");

            migrationBuilder.AddColumn<string>(
                name: "BlobPath",
                table: "InvoiceImage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "InvoiceImage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobPath",
                table: "InvoiceImage");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "InvoiceImage");

            migrationBuilder.RenameColumn(
                name: "MimeType",
                table: "InvoiceImage",
                newName: "Name");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "InvoiceImage",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: Array.Empty<byte>());
        }
    }
}
