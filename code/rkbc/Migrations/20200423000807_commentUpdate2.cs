using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class commentUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                 name: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                 name: "Comments");
        }
    }
}
