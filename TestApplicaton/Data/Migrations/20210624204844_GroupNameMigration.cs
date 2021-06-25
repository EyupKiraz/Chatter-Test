using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApplicaton.Data.Migrations
{
    public partial class GroupNameMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Group",
                table: "AspNetUsers",
                newName: "GroupId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "AspNetUsers",
                newName: "Group");
        }
    }
}
