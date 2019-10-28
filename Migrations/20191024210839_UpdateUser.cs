using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class UpdateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Function_EmployeeModel_EmployeeModelID",
                table: "Function");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_EmployeeModel_EmployeeModelID",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_EmployeeModel_EmployeeModelID",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_EmployeeModelID",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Locations_EmployeeModelID",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Function_EmployeeModelID",
                table: "Function");

            migrationBuilder.DropColumn(
                name: "EmployeeModelID",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "EmployeeModelID",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "EmployeeModelID",
                table: "Function");

            migrationBuilder.CreateTable(
                name: "EmployeeFunction",
                columns: table => new
                {
                    EmployeeFunctionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeID = table.Column<int>(nullable: false),
                    FunctionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeFunction", x => x.EmployeeFunctionID);
                    table.ForeignKey(
                        name: "FK_EmployeeFunction_EmployeeModel_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeFunction_Function_FunctionID",
                        column: x => x.FunctionID,
                        principalTable: "Function",
                        principalColumn: "FunctionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLocations",
                columns: table => new
                {
                    EmployeeLocationsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeModelID = table.Column<int>(nullable: false),
                    LocationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLocations", x => x.EmployeeLocationsID);
                    table.ForeignKey(
                        name: "FK_EmployeeLocations_EmployeeModel_EmployeeModelID",
                        column: x => x.EmployeeModelID,
                        principalTable: "EmployeeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeLocations_Locations_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSkill",
                columns: table => new
                {
                    EmployeeSkillID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeModelID = table.Column<int>(nullable: false),
                    SkillID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkill", x => x.EmployeeSkillID);
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_EmployeeModel_EmployeeModelID",
                        column: x => x.EmployeeModelID,
                        principalTable: "EmployeeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Skill_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skill",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFunction_EmployeeID",
                table: "EmployeeFunction",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeFunction_FunctionID",
                table: "EmployeeFunction",
                column: "FunctionID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLocations_EmployeeModelID",
                table: "EmployeeLocations",
                column: "EmployeeModelID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLocations_LocationID",
                table: "EmployeeLocations",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_EmployeeModelID",
                table: "EmployeeSkill",
                column: "EmployeeModelID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_SkillID",
                table: "EmployeeSkill",
                column: "SkillID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeFunction");

            migrationBuilder.DropTable(
                name: "EmployeeLocations");

            migrationBuilder.DropTable(
                name: "EmployeeSkill");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelID",
                table: "Skill",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelID",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeModelID",
                table: "Function",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skill_EmployeeModelID",
                table: "Skill",
                column: "EmployeeModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_EmployeeModelID",
                table: "Locations",
                column: "EmployeeModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Function_EmployeeModelID",
                table: "Function",
                column: "EmployeeModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Function_EmployeeModel_EmployeeModelID",
                table: "Function",
                column: "EmployeeModelID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_EmployeeModel_EmployeeModelID",
                table: "Locations",
                column: "EmployeeModelID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_EmployeeModel_EmployeeModelID",
                table: "Skill",
                column: "EmployeeModelID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
