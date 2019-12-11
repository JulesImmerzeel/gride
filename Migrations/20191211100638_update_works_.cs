using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class update_works_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Function_FunctionID",
                table: "Works");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionID",
                table: "Works",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Function_FunctionID",
                table: "Works",
                column: "FunctionID",
                principalTable: "Function",
                principalColumn: "FunctionID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_Function_FunctionID",
                table: "Works");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionID",
                table: "Works",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Function_FunctionID",
                table: "Works",
                column: "FunctionID",
                principalTable: "Function",
                principalColumn: "FunctionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
