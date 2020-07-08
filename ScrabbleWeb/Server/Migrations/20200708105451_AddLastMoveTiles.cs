using Microsoft.EntityFrameworkCore.Migrations;

namespace ScrabbleWeb.Server.Migrations
{
    public partial class AddLastMoveTiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastMoveTile",
                columns: table => new
                {
                    GameId = table.Column<int>(nullable: false),
                    TileId = table.Column<int>(nullable: false),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastMoveTile", x => new { x.GameId, x.TileId });
                    table.ForeignKey(
                        name: "FK_LastMoveTile_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastMoveTile");
        }
    }
}
