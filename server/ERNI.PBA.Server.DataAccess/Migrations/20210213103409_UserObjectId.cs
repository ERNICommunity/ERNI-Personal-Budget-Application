using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class UserObjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ObjectId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ObjectId",
                table: "Users",
                column: "ObjectId",
                unique: true,
                filter: "[ObjectId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ObjectId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "Users");
        }
    }
}
