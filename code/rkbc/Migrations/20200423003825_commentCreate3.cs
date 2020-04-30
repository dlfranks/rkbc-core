using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace rkbc.Migrations
{
    public partial class commentCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<string>(nullable: true),
                    postId = table.Column<int>(nullable: false),
                    content = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    isUser = table.Column<bool>(nullable: false),
                    pubDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_authorId",
                        column: x => x.authorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_postId",
                        column: x => x.postId,
                        principalTable: "Posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Comments_authorId",
                table: "Comments",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_postId",
                table: "Comments",
                column: "postId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
