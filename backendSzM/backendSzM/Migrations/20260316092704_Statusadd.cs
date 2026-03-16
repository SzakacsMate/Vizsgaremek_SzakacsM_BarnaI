using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class Statusadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Lobbies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lobbies");
        }
    }
}
