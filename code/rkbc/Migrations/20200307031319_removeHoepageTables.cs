using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class removeHoepageTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentItem");

            migrationBuilder.DropTable(
                name: "videoAttachments");

            migrationBuilder.DropTable(
                name: "homePages");

            migrationBuilder.DropTable(
                name: "attachments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    originalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pageId = table.Column<int>(type: "int", nullable: false),
                    sectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "videoAttachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    pageId = table.Column<int>(type: "int", nullable: false),
                    sectionId = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoAttachments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "homePages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bannerId = table.Column<int>(type: "int", nullable: false),
                    churchAnnounceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastUpdDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    lastUpdUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    memberAnnounceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pageId = table.Column<int>(type: "int", nullable: false),
                    schoolAnnounceTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sectionId = table.Column<int>(type: "int", nullable: false),
                    sundayServiceVideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    titleContent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_homePages", x => x.id);
                    table.ForeignKey(
                        name: "FK_homePages_attachments_bannerId",
                        column: x => x.bannerId,
                        principalTable: "attachments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentItem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomePageid = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    pageId = table.Column<int>(type: "int", nullable: false),
                    sectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentItem", x => x.id);
                    table.ForeignKey(
                        name: "FK_ContentItem_homePages_HomePageid",
                        column: x => x.HomePageid,
                        principalTable: "homePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItem_HomePageid",
                table: "ContentItem",
                column: "HomePageid");

            migrationBuilder.CreateIndex(
                name: "IX_homePages_bannerId",
                table: "homePages",
                column: "bannerId");
        }
    }
}
