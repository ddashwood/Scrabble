using Microsoft.EntityFrameworkCore.Migrations;

namespace ScrabbleWeb.Server.Migrations
{
    public partial class AddLastMoveDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastMoveDescription",
                table: "Game",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMoveDescription",
                table: "Game");
        }
    }
}
