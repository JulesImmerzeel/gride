using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class UpdateEmployeeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginID",
                table: "EmployeeModel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LoginID",
                table: "EmployeeModel",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
