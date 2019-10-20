using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class workfunctionKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Work_ShiftID_EmployeeID",
                table: "Work");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Work_EmployeeID_ShiftID_WorkID",
                table: "Work");

            migrationBuilder.AddColumn<int>(
                name: "FunctionID",
                table: "Work",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Work_ShiftID_EmployeeID_FunctionID",
                table: "Work",
                columns: new[] { "ShiftID", "EmployeeID", "FunctionID" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Work_EmployeeID_FunctionID_ShiftID_WorkID",
                table: "Work",
                columns: new[] { "EmployeeID", "FunctionID", "ShiftID", "WorkID" });

            migrationBuilder.CreateIndex(
                name: "IX_Work_FunctionID",
                table: "Work",
                column: "FunctionID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Work_Function_FunctionID",
                table: "Work",
                column: "FunctionID",
                principalTable: "Function",
                principalColumn: "FunctionID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Work_Function_FunctionID",
                table: "Work");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Work_ShiftID_EmployeeID_FunctionID",
                table: "Work");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Work_EmployeeID_FunctionID_ShiftID_WorkID",
                table: "Work");

            migrationBuilder.DropIndex(
                name: "IX_Work_FunctionID",
                table: "Work");

            migrationBuilder.DropColumn(
                name: "FunctionID",
                table: "Work");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Work_ShiftID_EmployeeID",
                table: "Work",
                columns: new[] { "ShiftID", "EmployeeID" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Work_EmployeeID_ShiftID_WorkID",
                table: "Work",
                columns: new[] { "EmployeeID", "ShiftID", "WorkID" });
        }
    }
}
