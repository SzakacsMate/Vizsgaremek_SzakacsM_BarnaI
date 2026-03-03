using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendSzM.Migrations
{
    /// <inheritdoc />
    public partial class Komment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Komments_Users_FogadoUserId",
                table: "Komments");

            migrationBuilder.DropForeignKey(
                name: "FK_Komments_Users_KommentaloUserId",
                table: "Komments");

            migrationBuilder.AlterColumn<string>(
                name: "AccesToken",
                table: "Tokens",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Komments_Users_FogadoUserId",
                table: "Komments",
                column: "FogadoUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Komments_Users_KommentaloUserId",
                table: "Komments",
                column: "KommentaloUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Komments_Users_FogadoUserId",
                table: "Komments");

            migrationBuilder.DropForeignKey(
                name: "FK_Komments_Users_KommentaloUserId",
                table: "Komments");

            migrationBuilder.AlterColumn<string>(
                name: "AccesToken",
                table: "Tokens",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Komments_Users_FogadoUserId",
                table: "Komments",
                column: "FogadoUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Komments_Users_KommentaloUserId",
                table: "Komments",
                column: "KommentaloUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
