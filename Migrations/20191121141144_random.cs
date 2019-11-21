using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class random : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunction_EmployeeModel_EmployeeID",
                table: "EmployeeFunction");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunction_Function_FunctionID",
                table: "EmployeeFunction");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkill_EmployeeModel_EmployeeModelID",
                table: "EmployeeSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkill_Skill_SkillID",
                table: "EmployeeSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSkill",
                table: "EmployeeSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeFunction",
                table: "EmployeeFunction");

            migrationBuilder.RenameTable(
                name: "EmployeeSkill",
                newName: "EmployeeSkills");

            migrationBuilder.RenameTable(
                name: "EmployeeFunction",
                newName: "EmployeeFunctions");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkill_SkillID",
                table: "EmployeeSkills",
                newName: "IX_EmployeeSkills_SkillID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkill_EmployeeModelID",
                table: "EmployeeSkills",
                newName: "IX_EmployeeSkills_EmployeeModelID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunction_FunctionID",
                table: "EmployeeFunctions",
                newName: "IX_EmployeeFunctions_FunctionID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunction_EmployeeID",
                table: "EmployeeFunctions",
                newName: "IX_EmployeeFunctions_EmployeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills",
                column: "EmployeeSkillID");

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
                name: "FK_EmployeeFunctions_EmployeeModel_EmployeeID",
                table: "EmployeeFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeFunctions_Function_FunctionID",
                table: "EmployeeFunctions");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkills_EmployeeModel_EmployeeModelID",
                table: "EmployeeSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSkills_Skill_SkillID",
                table: "EmployeeSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSkills",
                table: "EmployeeSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeFunctions",
                table: "EmployeeFunctions");

            migrationBuilder.RenameTable(
                name: "EmployeeSkills",
                newName: "EmployeeSkill");

            migrationBuilder.RenameTable(
                name: "EmployeeFunctions",
                newName: "EmployeeFunction");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkills_SkillID",
                table: "EmployeeSkill",
                newName: "IX_EmployeeSkill_SkillID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeSkills_EmployeeModelID",
                table: "EmployeeSkill",
                newName: "IX_EmployeeSkill_EmployeeModelID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunctions_FunctionID",
                table: "EmployeeFunction",
                newName: "IX_EmployeeFunction_FunctionID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeFunctions_EmployeeID",
                table: "EmployeeFunction",
                newName: "IX_EmployeeFunction_EmployeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSkill",
                table: "EmployeeSkill",
                column: "EmployeeSkillID");

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
