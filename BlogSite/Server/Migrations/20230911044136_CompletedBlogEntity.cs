using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogSite.Server.Migrations
{
    /// <inheritdoc />
    public partial class CompletedBlogEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "Blogs");
        }
    }
}
