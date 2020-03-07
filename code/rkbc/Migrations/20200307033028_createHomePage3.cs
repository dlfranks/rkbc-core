using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class createHomePage3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomePages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    createDt = table.Column<DateTime>(nullable: true),
                    createUser = table.Column<string>(nullable: true),
                    lastUpdDt = table.Column<DateTime>(nullable: true),
                    lastUpdUser = table.Column<string>(nullable: true),
                    bannerUrl = table.Column<string>(nullable: true),
                    bannerFileName = table.Column<string>(nullable: true),
                    originalFileName = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    titleContent = table.Column<string>(nullable: true),
                    churchAnnounceTitle = table.Column<string>(nullable: true),
                    memberAnnounceTitle = table.Column<string>(nullable: true),
                    schoolAnnounceTitle = table.Column<string>(nullable: true),
                    sundayServiceVideoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "HomeAttachments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    homePageId = table.Column<int>(nullable: false),
                    sectionId = table.Column<int>(nullable: false),
                    fileName = table.Column<string>(nullable: true),
                    originalFileName = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeAttachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeAttachments_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeContentItems",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    homePageId = table.Column<int>(nullable: false),
                    sectionId = table.Column<int>(nullable: false),
                    content = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeContentItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeContentItems_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeVideoAttachments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    homePageId = table.Column<int>(nullable: false),
                    sectionId = table.Column<int>(nullable: false),
                    url = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeVideoAttachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeVideoAttachments_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeAttachments_homePageId",
                table: "HomeAttachments",
                column: "homePageId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeContentItems_homePageId",
                table: "HomeContentItems",
                column: "homePageId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeVideoAttachments_homePageId",
                table: "HomeVideoAttachments",
                column: "homePageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeAttachments");

            migrationBuilder.DropTable(
                name: "HomeContentItems");

            migrationBuilder.DropTable(
                name: "HomeVideoAttachments");

            migrationBuilder.DropTable(
                name: "HomePages");
        }
    }
}
