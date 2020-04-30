using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class editPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "postType",
                table: "Posts",
                nullable: false,
                defaultValue: 1000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "postType",
                table: "Posts");
        }
    }
}
