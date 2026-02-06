using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BannedUsers",
                columns: table => new
                {
                    BanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsBanned = table.Column<float>(type: "REAL", nullable: false),
                    Warnings = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedUsers", x => x.BanId);
                });

            migrationBuilder.CreateTable(
                name: "Lobby",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Dm = table.Column<string>(type: "TEXT", nullable: false),
                    TtType = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    TimeDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlayerLimit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobby", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAuth",
                columns: table => new
                {
                    UserAuthId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuth", x => x.UserAuthId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    Gmail = table.Column<string>(type: "TEXT", nullable: false),
                    Rep = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BannedId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_BannedUsers_BannedId",
                        column: x => x.BannedId,
                        principalTable: "BannedUsers",
                        principalColumn: "BanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserAuth_AuthId",
                        column: x => x.AuthId,
                        principalTable: "UserAuth",
                        principalColumn: "UserAuthId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LobbyCon",
                columns: table => new
                {
                    LobbyConId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LobbyId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobbyCon", x => x.LobbyConId);
                    table.ForeignKey(
                        name: "FK_LobbyCon_Lobby_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobby",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LobbyCon_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LobbyCon_LobbyId",
                table: "LobbyCon",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_LobbyCon_UserId",
                table: "LobbyCon",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthId",
                table: "Users",
                column: "AuthId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BannedId",
                table: "Users",
                column: "BannedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LobbyCon");

            migrationBuilder.DropTable(
                name: "Lobby");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BannedUsers");

            migrationBuilder.DropTable(
                name: "UserAuth");
        }
    }
}
