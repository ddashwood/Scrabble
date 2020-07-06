using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScrabbleWeb.Server.Migrations
{
    public partial class AddFieldsForRecentGamesList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "Game",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMove",
                table: "Game",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Winner",
                table: "Game",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "LastMove",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "Winner",
                table: "Game");
        }
    }
}
