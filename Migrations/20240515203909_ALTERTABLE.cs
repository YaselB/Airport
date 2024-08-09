using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aeropuerto.Migrations
{
    /// <inheritdoc />
    public partial class ALTERTABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArriveTime",
                table: "Flights",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "dateTime",
                table: "Flights",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "departureTime",
                table: "Flights",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArriveTime",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "dateTime",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "departureTime",
                table: "Flights");
        }
    }
}
