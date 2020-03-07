using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class updateHomepage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeAttachment");

            migrationBuilder.DropTable(
                name: "HomeBannerAttachment");

            migrationBuilder.DropTable(
                name: "HomeItem");

            migrationBuilder.DropTable(
                name: "HomeVideoAttachment");

            migrationBuilder.DropColumn(
                name: "churchAnnouncTitle",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "memberAnnouncTitle",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "schoolAnnouncTitle",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "siteId",
                table: "homePages");

            migrationBuilder.AddColumn<int>(
                name: "bannerId",
                table: "homePages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "churchAnnounceTitle",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "createDt",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "createUser",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdDt",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lastUpdUser",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "memberAnnounceTitle",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "schoolAnnounceTitle",
                table: "homePages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sectionId",
                table: "homePages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "sundayServiceVideoUrl",
                table: "homePages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sectionId = table.Column<int>(nullable: false),
                    pageId = table.Column<int>(nullable: false),
                    fileName = table.Column<string>(nullable: true),
                    originalFileName = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ContentItem",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sectionId = table.Column<int>(nullable: false),
                    pageId = table.Column<int>(nullable: false),
                    content = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false),
                    HomePageid = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "videoAttachments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sectionId = table.Column<int>(nullable: false),
                    pageId = table.Column<int>(nullable: false),
                    url = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_videoAttachments", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_homePages_bannerId",
                table: "homePages",
                column: "bannerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItem_HomePageid",
                table: "ContentItem",
                column: "HomePageid");

            migrationBuilder.AddForeignKey(
                name: "FK_homePages_attachments_bannerId",
                table: "homePages",
                column: "bannerId",
                principalTable: "attachments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_homePages_attachments_bannerId",
                table: "homePages");

            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "ContentItem");

            migrationBuilder.DropTable(
                name: "videoAttachments");

            migrationBuilder.DropIndex(
                name: "IX_homePages_bannerId",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "bannerId",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "churchAnnounceTitle",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "createDt",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "createUser",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "lastUpdDt",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "lastUpdUser",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "memberAnnounceTitle",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "schoolAnnounceTitle",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "sectionId",
                table: "homePages");

            migrationBuilder.DropColumn(
                name: "sundayServiceVideoUrl",
                table: "homePages");

            migrationBuilder.AddColumn<string>(
                name: "churchAnnouncTitle",
                table: "homePages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "memberAnnouncTitle",
                table: "homePages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "schoolAnnouncTitle",
                table: "homePages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "siteId",
                table: "homePages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HomeAttachment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    homePageId = table.Column<int>(type: "int", nullable: false),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    isPdf = table.Column<bool>(type: "bit", nullable: false),
                    sectionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeAttachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeAttachment_homePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "homePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeBannerAttachment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    homePageId = table.Column<int>(type: "int", nullable: false),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    isPdf = table.Column<bool>(type: "bit", nullable: false),
                    sectionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeBannerAttachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeBannerAttachment_homePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "homePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeItem",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    homePageId = table.Column<int>(type: "int", nullable: false),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    sectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeItem", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeItem_homePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "homePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeVideoAttachment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    homePageId = table.Column<int>(type: "int", nullable: false),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    sectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeVideoAttachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeVideoAttachment_homePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "homePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeAttachment_homePageId",
                table: "HomeAttachment",
                column: "homePageId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeBannerAttachment_homePageId",
                table: "HomeBannerAttachment",
                column: "homePageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomeItem_homePageId",
                table: "HomeItem",
                column: "homePageId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeVideoAttachment_homePageId",
                table: "HomeVideoAttachment",
                column: "homePageId");
        }
    }
}
