using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class LobbyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerMin",
                table: "Lobbies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerMin",
                table: "Lobbies");
        }
    }
}
