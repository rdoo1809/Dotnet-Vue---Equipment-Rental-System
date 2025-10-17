using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Midterm_PROG3340_RDooley.Migrations
{
    /// <inheritdoc />
    public partial class RentalMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rental",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rental", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1,
                column: "Role",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2,
                column: "Role",
                value: "User");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Role",
                value: "User");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4,
                column: "Role",
                value: "User");

            migrationBuilder.InsertData(
                table: "Rental",
                columns: new[] { "Id", "CustomerId", "EquipmentId", "IssuedAt", "ReturnedAt" },
                values: new object[,]
                {
                    { 1, 2, 3, new DateTime(2025, 10, 1, 6, 0, 0, 0, DateTimeKind.Local), null },
                    { 2, 3, 5, new DateTime(2025, 9, 15, 10, 0, 0, 0, DateTimeKind.Local), new DateTime(2025, 9, 20, 12, 0, 0, 0, DateTimeKind.Local) },
                    { 3, 4, 7, new DateTime(2025, 9, 10, 5, 30, 0, 0, DateTimeKind.Local), new DateTime(2025, 9, 15, 8, 0, 0, 0, DateTimeKind.Local) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rental");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1,
                column: "Role",
                value: "amdin");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2,
                column: "Role",
                value: "user");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Role",
                value: "user");

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 4,
                column: "Role",
                value: "user");
        }
    }
}
