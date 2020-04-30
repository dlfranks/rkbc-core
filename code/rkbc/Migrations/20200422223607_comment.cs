using Microsoft.EntityFrameworkCore.Migrations;

namespace rkbc.Migrations
{
    public partial class comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_authorId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "isAdmin",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "authorId",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "isUser",
                table: "Comments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_authorId",
                table: "Comments",
                column: "authorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_authorId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "isUser",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "authorId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAdmin",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_authorId",
                table: "Comments",
                column: "authorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
