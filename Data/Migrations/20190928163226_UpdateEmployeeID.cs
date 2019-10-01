using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Data.Migrations
{
    public partial class UpdateEmployeeID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropPrimaryKey("PK_EmployeeModel", "EmployeeModel");
			migrationBuilder.DropColumn("ID", "EmployeeModel");
			migrationBuilder.AddColumn<long>("ID", "EmployeeModel", defaultValue: 0L);
			migrationBuilder.AddPrimaryKey("PK_EmployeeModel", "EmployeeModel", "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropPrimaryKey("PK_EmployeeModel", "EmployeeModel");
			migrationBuilder.DropColumn("ID", "EmployeeModel");
			migrationBuilder.AddColumn<long>("ID", "EmployeeModel", defaultValue: 0L)
				.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
			migrationBuilder.AddPrimaryKey("PK_EmployeeModel", "EmployeeModel", "ID");
		}
	}
}
