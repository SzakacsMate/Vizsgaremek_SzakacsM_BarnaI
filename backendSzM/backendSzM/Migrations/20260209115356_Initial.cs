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
                name: "Auths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannedUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsBanned = table.Column<float>(type: "REAL", nullable: false),
                    Warnings = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lobbies",
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
                    table.PrimaryKey("PK_Lobbies", x => x.Id);
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
                    BannedId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BannedUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AuthId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserAuthId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Auths_UserAuthId",
                        column: x => x.UserAuthId,
                        principalTable: "Auths",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_BannedUsers_BannedUserId",
                        column: x => x.BannedUserId,
                        principalTable: "BannedUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LobbyCons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LobbyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserDataId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobbyCons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LobbyCons_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LobbyCons_Users_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LobbyCons_LobbyId",
                table: "LobbyCons",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_LobbyCons_UserDataId",
                table: "LobbyCons",
                column: "UserDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BannedUserId",
                table: "Users",
                column: "BannedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserAuthId",
                table: "Users",
                column: "UserAuthId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LobbyCons");

            migrationBuilder.DropTable(
                name: "Lobbies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Auths");

            migrationBuilder.DropTable(
                name: "BannedUsers");
        }
    }
}
