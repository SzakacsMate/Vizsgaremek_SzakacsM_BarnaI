using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class KommentCon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Komments_Users_UserId",
                table: "Komments");

            migrationBuilder.DropIndex(
                name: "IX_Komments_UserId",
                table: "Komments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Komments",
                newName: "KommentaloId");

            migrationBuilder.AddColumn<Guid>(
                name: "FogadoId",
                table: "Komments",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "KommentCons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    komment = table.Column<Guid>(type: "TEXT", nullable: false),
                    KommentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserDataId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KommentCons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KommentCons_Komments_KommentId",
                        column: x => x.KommentId,
                        principalTable: "Komments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KommentCons_Users_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KommentCons_KommentId",
                table: "KommentCons",
                column: "KommentId");

            migrationBuilder.CreateIndex(
                name: "IX_KommentCons_UserDataId",
                table: "KommentCons",
                column: "UserDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KommentCons");

            migrationBuilder.DropColumn(
                name: "FogadoId",
                table: "Komments");

            migrationBuilder.RenameColumn(
                name: "KommentaloId",
                table: "Komments",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Komments_UserId",
                table: "Komments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Komments_Users_UserId",
                table: "Komments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
