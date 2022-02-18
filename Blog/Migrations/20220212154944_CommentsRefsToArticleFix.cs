using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Migrations
{
    public partial class CommentsRefsToArticleFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Article_ArticleViewModelId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ArticleViewModelId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ArticleViewModelId",
                table: "Comment");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ArticleId",
                table: "Comment",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Article_ArticleId",
                table: "Comment",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Article_ArticleId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ArticleId",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "ArticleViewModelId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ArticleViewModelId",
                table: "Comment",
                column: "ArticleViewModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Article_ArticleViewModelId",
                table: "Comment",
                column: "ArticleViewModelId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
