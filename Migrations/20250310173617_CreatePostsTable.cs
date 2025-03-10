using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CroundFundingProject.Migrations
{
    /// <inheritdoc />
    public partial class CreatePostsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "title",
                table: "Posts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Posts",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Posts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "media_url",
                table: "Posts",
                newName: "MediaUrl");

            migrationBuilder.RenameColumn(
                name: "amount_gained",
                table: "Posts",
                newName: "AmountGained");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<int>(type: "int", nullable: false),
                    postId = table.Column<int>(type: "int", nullable: false),
                    text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Posts",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Posts",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Posts",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "Posts",
                newName: "media_url");

            migrationBuilder.RenameColumn(
                name: "AmountGained",
                table: "Posts",
                newName: "amount_gained");
        }
    }
}
