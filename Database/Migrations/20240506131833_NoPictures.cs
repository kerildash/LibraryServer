using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class NoPictures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Books_HolderId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_HolderId",
                table: "Pictures");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pictures_HolderId",
                table: "Pictures",
                column: "HolderId",
                unique: true,
                filter: "[HolderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Books_HolderId",
                table: "Pictures",
                column: "HolderId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
