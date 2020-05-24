using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class UserActivityLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    activityDate = table.Column<DateTime>(nullable: false),
                    username = table.Column<string>(nullable: true),
                    firstname = table.Column<string>(nullable: true),
                    lastname = table.Column<string>(nullable: true),
                    activity = table.Column<string>(nullable: true),
                    isMobileInterface = table.Column<bool>(nullable: false),
                    userAgent = table.Column<string>(nullable: true),
                    offices = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DBAudits",
                table: "DBAudits");

            migrationBuilder.RenameTable(
                name: "DBAudits",
                newName: "audit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit",
                table: "audit",
                column: "id");
        }
    }
}
