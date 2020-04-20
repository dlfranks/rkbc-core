using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class pastorModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeAttachments");

            migrationBuilder.DropTable(
                name: "VideoAttachments");

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pageEnum = table.Column<int>(nullable: false),
                    attachmentSectionEnum = table.Column<int>(nullable: false),
                    createDt = table.Column<DateTime>(nullable: true),
                    createUser = table.Column<string>(nullable: true),
                    lastUpdDt = table.Column<DateTime>(nullable: true),
                    lastUpdUser = table.Column<string>(nullable: true),
                    fileName = table.Column<string>(nullable: true),
                    originalFileName = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.CreateTable(
                name: "HomeAttachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    homePageId = table.Column<int>(type: "int", nullable: false),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    lastUpdDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    lastUpdUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    originalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sectionId = table.Column<int>(type: "int", nullable: false)
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
                name: "VideoAttachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    homePageId = table.Column<int>(type: "int", nullable: false),
                    isOn = table.Column<bool>(type: "bit", nullable: false),
                    sectionId = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoAttachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_VideoAttachments_HomePages_homePageId",
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
                name: "IX_VideoAttachments_homePageId",
                table: "VideoAttachments",
                column: "homePageId");
        }
    }
}
