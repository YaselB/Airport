using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class Corrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_User_UserCI",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "UserCI",
                table: "Reservations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_User_UserCI",
                table: "Reservations",
                column: "UserCI",
                principalTable: "User",
                principalColumn: "CI",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_User_UserCI",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "UserCI",
                table: "Reservations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_User_UserCI",
                table: "Reservations",
                column: "UserCI",
                principalTable: "User",
                principalColumn: "CI");
        }
    }
}
