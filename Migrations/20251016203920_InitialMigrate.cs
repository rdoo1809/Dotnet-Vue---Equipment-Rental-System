using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Midterm_PROG3340_RDooley.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    RentalPrice = table.Column<double>(type: "REAL", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsAvailable", "Name", "RentalPrice" },
                values: new object[,]
                {
                    { 1, "Heavy Machinery", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Large hydraulic excavator for heavy lifting", true, "Excavator", 250.0 },
                    { 2, "Power Tools", new DateTime(2022, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "18V battery-powered drill for versatile use", true, "Cordless Drill", 15.0 },
                    { 3, "Vehicles", new DateTime(2021, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "4x4 truck for transporting equipment", false, "Pickup Truck", 80.0 },
                    { 4, "Safety", new DateTime(2024, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "High-impact protective helmet", true, "Safety Helmet", 5.0 },
                    { 5, "Surveying", new DateTime(2023, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Precision surveying instrument for angle measurements", false, "Theodolite", 40.0 },
                    { 6, "Power Tools", new DateTime(2022, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gas-powered chainsaw for tree cutting", true, "Chainsaw", 25.0 },
                    { 7, "Heavy Machinery", new DateTime(2023, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Electric forklift for warehouse operations", true, "Forklift", 120.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
