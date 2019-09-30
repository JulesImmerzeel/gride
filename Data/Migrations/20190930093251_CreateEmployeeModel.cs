using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Data.Migrations
{
    public partial class CreateEmployeeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeModel",
                columns: table => new
                {
                    EmployeeID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    DoB = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    EMail = table.Column<string>(maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 12, nullable: false),
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
                    table.PrimaryKey("PK_EmployeeModel", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "SkillModel",
                columns: table => new
                {
                    SkillID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillModel", x => x.SkillID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeModel");

            migrationBuilder.DropTable(
                name: "SkillModel");
        }
    }
}
