using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class ShiftWeeklyBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Weekly",
                table: "Shift",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weekly",
                table: "Shift");
        }
    }
}
