using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CroundFundingProject.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultCreatedAtForBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_Posts_PostId",
                table: "Donations");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_Users_UserId",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Donations_PostId",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Donations_UserId",
                table: "Donations");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Posts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "BankAccounts",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Posts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "BankAccounts",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_PostId",
                table: "Donations",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_UserId",
                table: "Donations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_Posts_PostId",
                table: "Donations",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_Users_UserId",
                table: "Donations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
