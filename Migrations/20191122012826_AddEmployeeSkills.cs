using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class AddEmployeeSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkill_EmployeeModel_EmployeeModelID",
                table: "EmployeeSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkill_Skill_SkillID",
                table: "EmployeeSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSkill",
                table: "EmployeeSkill");

            migrationBuilder.RenameTable(
                name: "EmployeeSkill",
                newName: "EmployeeSkills");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkill_SkillID",
                table: "EmployeeSkills",
                newName: "IX_EmployeeSkills_SkillID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkill_EmployeeModelID",
                table: "EmployeeSkills",
                newName: "IX_EmployeeSkills_EmployeeModelID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills",
                column: "EmployeeSkillID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSkills_EmployeeModel_EmployeeModelID",
                table: "EmployeeSkills",
                column: "EmployeeModelID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSkills_Skill_SkillID",
                table: "EmployeeSkills",
                column: "SkillID",
                principalTable: "Skill",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkills_EmployeeModel_EmployeeModelID",
                table: "EmployeeSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkills_Skill_SkillID",
                table: "EmployeeSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills");

            migrationBuilder.RenameTable(
                name: "EmployeeSkills",
                newName: "EmployeeSkill");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkills_SkillID",
                table: "EmployeeSkill",
                newName: "IX_EmployeeSkill_SkillID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkills_EmployeeModelID",
                table: "EmployeeSkill",
                newName: "IX_EmployeeSkill_EmployeeModelID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSkill",
                table: "EmployeeSkill",
                column: "EmployeeSkillID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSkill_EmployeeModel_EmployeeModelID",
                table: "EmployeeSkill",
                column: "EmployeeModelID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSkill_Skill_SkillID",
                table: "EmployeeSkill",
                column: "SkillID",
                principalTable: "Skill",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
