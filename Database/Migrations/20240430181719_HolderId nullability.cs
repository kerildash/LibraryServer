using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class HolderIdnullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Books_HolderId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Books_HolderId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_HolderId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Documents_HolderId",
                table: "Documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "HolderId",
                table: "Pictures",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "HolderId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_HolderId",
                table: "Pictures",
                column: "HolderId",
                unique: true,
                filter: "[HolderId] IS NOT NULL");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Books_HolderId",
                table: "Pictures",
                column: "HolderId",
                principalTable: "Books",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Books_HolderId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Pictures_Books_HolderId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_HolderId",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Documents_HolderId",
                table: "Documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "HolderId",
                table: "Pictures",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HolderId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_HolderId",
                table: "Pictures",
                column: "HolderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_HolderId",
                table: "Documents",
                column: "HolderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Books_HolderId",
                table: "Documents",
                column: "HolderId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pictures_Books_HolderId",
                table: "Pictures",
                column: "HolderId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
