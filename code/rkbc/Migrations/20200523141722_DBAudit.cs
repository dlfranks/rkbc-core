using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class DBAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DBAudits",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    action = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    user = table.Column<string>(nullable: true),
                    recordTypeName = table.Column<string>(nullable: true),
                    singleId = table.Column<string>(nullable: true),
                    complexId = table.Column<string>(nullable: true),
                    recordData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBAudits", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DBAudits");
        }
    }
}
