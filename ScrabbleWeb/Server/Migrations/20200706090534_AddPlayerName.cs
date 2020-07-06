using Microsoft.EntityFrameworkCore.Migrations;

namespace ScrabbleWeb.Server.Migrations
{
    public partial class AddPlayerName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Player",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Player");
        }
    }
}
