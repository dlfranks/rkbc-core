using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class creteHomePages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomePages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    siteId = table.Column<int>(nullable: false),
                    pageId = table.Column<int>(nullable: false),
                    title = table.Column<string>(nullable: true),
                    titleContent = table.Column<string>(nullable: true),
                    churchAnnouncTitle = table.Column<string>(nullable: true),
                    memberAnnouncTitle = table.Column<string>(nullable: true),
                    schoolAnnouncTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "HomeAttachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    homePageId = table.Column<int>(nullable: false),
                    sectionId = table.Column<string>(nullable: true),
                    fileName = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isPdf = table.Column<bool>(nullable: false),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeAttachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeAttachment_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeBannerAttachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    homePageId = table.Column<int>(nullable: false),
                    sectionId = table.Column<string>(nullable: true),
                    fileName = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isPdf = table.Column<bool>(nullable: false),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeBannerAttachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeBannerAttachment_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeItem",
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
                    table.PrimaryKey("PK_HomeItem", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeItem_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeVideoAttachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    homePageId = table.Column<int>(nullable: false),
                    sectionId = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    caption = table.Column<string>(nullable: true),
                    isOn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeVideoAttachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_HomeVideoAttachment_HomePages_homePageId",
                        column: x => x.homePageId,
                        principalTable: "HomePages",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
