using Microsoft.EntityFrameworkCore.Migrations;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class RemoveUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSuperior",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsViewer",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperior",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsViewer",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
