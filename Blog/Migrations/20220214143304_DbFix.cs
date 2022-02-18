using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Migrations
{
    public partial class DbFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Comment",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Article",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Article",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
