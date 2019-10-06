using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Data.Migrations
{
    public partial class EmployeeController : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeModel",
                columns: table => new
                {
                    EmployeeModelID = table.Column<long>(nullable: false),
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
                    table.PrimaryKey("PK_EmployeeModel", x => x.EmployeeModelID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeModel");
        }
    }
}
