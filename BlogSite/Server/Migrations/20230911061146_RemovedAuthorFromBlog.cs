using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogSite.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAuthorFromBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategory_AspNetUsers_CreatedById",
                table: "BlogCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_BlogCategory_CategoryId",
                table: "Blogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogCategory",
                table: "BlogCategory");

            migrationBuilder.DropIndex(
                name: "IX_BlogCategory_CreatedById",
                table: "BlogCategory");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BlogCategory");

            migrationBuilder.RenameTable(
                name: "BlogCategory",
                newName: "BlogCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogCategories",
                table: "BlogCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_BlogCategories_CategoryId",
                table: "Blogs",
                column: "CategoryId",
                principalTable: "BlogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_BlogCategories_CategoryId",
                table: "Blogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogCategories",
                table: "BlogCategories");

            migrationBuilder.RenameTable(
                name: "BlogCategories",
                newName: "BlogCategory");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "BlogCategory",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogCategory",
                table: "BlogCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategory_CreatedById",
                table: "BlogCategory",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategory_AspNetUsers_CreatedById",
                table: "BlogCategory",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_BlogCategory_CategoryId",
                table: "Blogs",
                column: "CategoryId",
                principalTable: "BlogCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
