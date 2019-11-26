using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class AddEmployeeFunctions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunction_EmployeeModel_EmployeeID",
                table: "EmployeeFunction");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunction_Function_FunctionID",
                table: "EmployeeFunction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeFunction",
                table: "EmployeeFunction");

            migrationBuilder.RenameTable(
                name: "EmployeeFunction",
                newName: "EmployeeFunctions");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunction_FunctionID",
                table: "EmployeeFunctions",
                newName: "IX_EmployeeFunctions_FunctionID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunction_EmployeeID",
                table: "EmployeeFunctions",
                newName: "IX_EmployeeFunctions_EmployeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeFunctions",
                table: "EmployeeFunctions",
                column: "EmployeeFunctionID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeFunctions_EmployeeModel_EmployeeID",
                table: "EmployeeFunctions",
                column: "EmployeeID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeFunctions_Function_FunctionID",
                table: "EmployeeFunctions",
                column: "FunctionID",
                principalTable: "Function",
                principalColumn: "FunctionID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunctions_EmployeeModel_EmployeeID",
                table: "EmployeeFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunctions_Function_FunctionID",
                table: "EmployeeFunctions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeFunctions",
                table: "EmployeeFunctions");

            migrationBuilder.RenameTable(
                name: "EmployeeFunctions",
                newName: "EmployeeFunction");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunctions_FunctionID",
                table: "EmployeeFunction",
                newName: "IX_EmployeeFunction_FunctionID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunctions_EmployeeID",
                table: "EmployeeFunction",
                newName: "IX_EmployeeFunction_EmployeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeFunction",
                table: "EmployeeFunction",
                column: "EmployeeFunctionID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeFunction_EmployeeModel_EmployeeID",
                table: "EmployeeFunction",
                column: "EmployeeID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeFunction_Function_FunctionID",
                table: "EmployeeFunction",
                column: "FunctionID",
                principalTable: "Function",
                principalColumn: "FunctionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
