using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Data.Migrations
{
    public partial class changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillModel",
                table: "SkillModel");

            migrationBuilder.RenameTable(
                name: "SkillModel",
                newName: "Skill");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeModelID",
                table: "Skill",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                table: "Skill",
                column: "SkillID");

			migrationBuilder.DropTable("EmployeeModel");

            migrationBuilder.CreateTable(
                name: "EmployeeModel",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    DoB = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    EMail = table.Column<string>(maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 12, nullable: false),
                    Admin = table.Column<bool>(nullable: false),
                    Function = table.Column<int>(nullable: false),
                    LoginID = table.Column<decimal>(nullable: false),
                    Experience = table.Column<float>(nullable: false),
                    Locations = table.Column<long>(nullable: false),
                    ProfileImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Function",
                columns: table => new
                {
                    FunctionID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EmployeeModelID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Function", x => x.FunctionID);
                    table.ForeignKey(
                        name: "FK_Function_EmployeeModel_EmployeeModelID",
                        column: x => x.EmployeeModelID,
                        principalTable: "EmployeeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_EmployeeModelID",
                table: "Skill",
                column: "EmployeeModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Function_EmployeeModelID",
                table: "Function",
                column: "EmployeeModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_EmployeeModel_EmployeeModelID",
                table: "Skill",
                column: "EmployeeModelID",
                principalTable: "EmployeeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_EmployeeModel_EmployeeModelID",
                table: "Skill");

            migrationBuilder.DropTable(
                name: "Function");

            migrationBuilder.DropTable(
                name: "EmployeeModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                table: "Skill");

            migrationBuilder.DropIndex(
                name: "IX_Skill_EmployeeModelID",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "EmployeeModelID",
                table: "Skill");

            migrationBuilder.RenameTable(
                name: "Skill",
                newName: "SkillModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillModel",
                table: "SkillModel",
                column: "SkillID");

			migrationBuilder.CreateTable(
				name: "EmployeeModel",
				columns: table => new
				{
					ID = table.Column<long>(nullable: false),
					Name = table.Column<string>(nullable: true),
					LastName = table.Column<string>(nullable: true),
					DoB = table.Column<DateTime>(nullable: false),
					Gender = table.Column<int>(nullable: false),
					EMail = table.Column<string>(nullable: true),
					PhoneNumber = table.Column<string>(nullable: true),
					Admin = table.Column<bool>(nullable: false),
					Skills = table.Column<decimal>(nullable: false),
					Function = table.Column<int>(nullable: false),
					LoginID = table.Column<decimal>(nullable: false),
					Experience = table.Column<float>(nullable: false),
					Locations = table.Column<long>(nullable: false),
					ProfileImage = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_EmployeeModel", x => x.ID);
				});
		}
    }
}
