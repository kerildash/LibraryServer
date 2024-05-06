using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class NoDocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Books_HolderId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_HolderId",
                table: "Documents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Documents_HolderId",
                table: "Documents",
                column: "HolderId",
                unique: true,
                filter: "[HolderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Books_HolderId",
                table: "Documents",
                column: "HolderId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
