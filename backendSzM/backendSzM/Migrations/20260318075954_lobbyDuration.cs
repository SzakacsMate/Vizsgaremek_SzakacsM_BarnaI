using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class lobbyDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Lobbies");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Lobbies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Lobbies");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Lobbies",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
