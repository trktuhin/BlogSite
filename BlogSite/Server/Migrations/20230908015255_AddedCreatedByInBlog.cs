using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogSite.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedByInBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CreatedById",
                table: "Blogs",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_CreatedById",
                table: "Blogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_CreatedById",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CreatedById",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Blogs");
        }
    }
}
