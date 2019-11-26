using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class AddEmployeeSupervisorID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisorID",
                table: "EmployeeModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupervisorID",
                table: "EmployeeModel");
        }
    }
}
