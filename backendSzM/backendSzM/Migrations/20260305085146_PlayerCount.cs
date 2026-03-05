using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class PlayerCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerCount",
                table: "Lobbies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerCount",
                table: "Lobbies");
        }
    }
}
