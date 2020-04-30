using Microsoft.EntityFrameworkCore.Migrations;
using rkbc.core.models;

namespace rkbc.Migrations
{
    public partial class AddAccountType3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "accountType",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: (int)AccountType.Personal);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accountType",
                table: "AspNetUsers");
        }
    }
}
