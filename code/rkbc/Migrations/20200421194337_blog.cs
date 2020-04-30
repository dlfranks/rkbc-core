using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class blog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<string>(nullable: true),
                    createDt = table.Column<DateTime>(nullable: false),
                    lastUpdDt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_Blogs_AspNetUsers_authorId",
                        column: x => x.authorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blogId = table.Column<int>(nullable: false),
                    imageFileName = table.Column<string>(nullable: true),
                    videoURL = table.Column<string>(nullable: true),
                    content = table.Column<string>(nullable: false),
                    excerpt = table.Column<string>(nullable: false),
                    isPublished = table.Column<bool>(nullable: false),
                    createDt = table.Column<DateTime>(nullable: false),
                    lastModified = table.Column<DateTime>(nullable: false),
                    pubDate = table.Column<DateTime>(nullable: false),
                    slug = table.Column<string>(nullable: true),
                    views = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_Posts_Blogs_blogId",
                        column: x => x.blogId,
                        principalTable: "Blogs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    authorId = table.Column<string>(nullable: false),
                    postId = table.Column<int>(nullable: false),
                    content = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: false),
                    isAdmin = table.Column<bool>(nullable: false),
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
                name: "IX_Blogs_authorId",
                table: "Blogs",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_authorId",
                table: "Comments",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_postId",
                table: "Comments",
                column: "postId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_blogId",
                table: "Posts",
                column: "blogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Blogs");
        }
    }
}
