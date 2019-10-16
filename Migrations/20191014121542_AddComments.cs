using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gride.Migrations
{
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_MessageID1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessageID1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageID1",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 2000, nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    EmployeeID = table.Column<int>(nullable: true),
                    MessageID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comments_EmployeeModel_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "MessageID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_EmployeeID",
                table: "Comments",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MessageID",
                table: "Comments",
                column: "MessageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "MessageID1",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageID1",
                table: "Messages",
                column: "MessageID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_MessageID1",
                table: "Messages",
                column: "MessageID1",
                principalTable: "Messages",
                principalColumn: "MessageID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
