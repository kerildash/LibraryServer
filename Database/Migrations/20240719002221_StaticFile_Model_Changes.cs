using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class StaticFile_Model_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Documents");

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                table: "Authors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_PhotoId",
                table: "Authors",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Pictures_PhotoId",
                table: "Authors",
                column: "PhotoId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Pictures_PhotoId",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_PhotoId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Authors");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
