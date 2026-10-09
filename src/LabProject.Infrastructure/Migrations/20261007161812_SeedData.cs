using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LabProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("a3d2e5b1-285e-4c8d-9b5f-8c38a2e7c001"), "Electronic devices and digital equipment", "Electronics" },
                    { new Guid("b4e3f6c2-396f-4d9e-0c6a-9d49b3f8d002"), "Technical literature and textbooks", "Books" },
                    { new Guid("c5f4a7d3-407a-4eaf-1d7b-0e5ac4a9e003"), "Peripherals, cables, and hardware components", "Accessories" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("a3d2e5b1-285e-4c8d-9b5f-8c38a2e7c001"), "Mechanical Keyboard", 89.99m, 25 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("a3d2e5b1-285e-4c8d-9b5f-8c38a2e7c001"), "Gaming Mouse", 49.50m, 40 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("b4e3f6c2-396f-4d9e-0c6a-9d49b3f8d002"), "CLR via C# by Jeffrey Richter", 54.00m, 15 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("c5f4a7d3-407a-4eaf-1d7b-0e5ac4a9e003"), "USB-C to DisplayPort Cable", 18.25m, 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a3d2e5b1-285e-4c8d-9b5f-8c38a2e7c001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b4e3f6c2-396f-4d9e-0c6a-9d49b3f8d002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c5f4a7d3-407a-4eaf-1d7b-0e5ac4a9e003"));
        }
    }
}
