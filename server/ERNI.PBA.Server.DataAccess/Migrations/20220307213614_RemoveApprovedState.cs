using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    public partial class RemoveApprovedState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE Requests SET State = CASE 
            WHEN State = 0 THEN 0
            WHEN State = 1 THEN 0
            WHEN State = 2 THEN 1
            ELSE 2
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
