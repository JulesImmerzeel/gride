using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Data.Migrations
{
    public partial class EmployeeID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeModel",
                table: "EmployeeModel");

            migrationBuilder.DropColumn(
                name: "EmployeeModelID",
                table: "EmployeeModel");

            migrationBuilder.AddColumn<long>(
                name: "ID",
                table: "EmployeeModel",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeModel",
                table: "EmployeeModel",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeModel",
                table: "EmployeeModel");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "EmployeeModel");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeModelID",
                table: "EmployeeModel",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeModel",
                table: "EmployeeModel",
                column: "EmployeeModelID");
        }
    }
}
