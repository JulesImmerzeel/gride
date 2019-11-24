using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class WeeklyShift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentShiftID",
                table: "Shift",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShiftID1",
                table: "Shift",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shift_ShiftID1",
                table: "Shift",
                column: "ShiftID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Shift_Shift_ShiftID1",
                table: "Shift",
                column: "ShiftID1",
                principalTable: "Shift",
                principalColumn: "ShiftID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shift_Shift_ShiftID1",
                table: "Shift");

            migrationBuilder.DropIndex(
                name: "IX_Shift_ShiftID1",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "ParentShiftID",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "ShiftID1",
                table: "Shift");
        }
    }
}
