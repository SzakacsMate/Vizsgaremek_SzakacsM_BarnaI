using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class Mod1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeDate",
                table: "Lobbies",
                newName: "StartDate");

            migrationBuilder.AddColumn<string>(
                name: "ProfileI",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Lobbies",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileI",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Lobbies");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Lobbies",
                newName: "TimeDate");
        }
    }
}
