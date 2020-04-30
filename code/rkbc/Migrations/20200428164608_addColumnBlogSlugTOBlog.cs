using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class addColumnBlogSlugTOBlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blogSlug",
                table: "Blogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blogSlug",
                table: "Blogs");
        }
    }
}
