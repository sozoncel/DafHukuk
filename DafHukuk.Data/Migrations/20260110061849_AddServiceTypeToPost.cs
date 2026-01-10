using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DafHukuk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTypeToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 10, 6, 18, 49, 40, DateTimeKind.Utc).AddTicks(1087));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 10, 6, 18, 49, 40, DateTimeKind.Utc).AddTicks(1089));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 10, 6, 18, 49, 40, DateTimeKind.Utc).AddTicks(1090));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Posts");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 29, 14, 24, 37, 566, DateTimeKind.Utc).AddTicks(3349));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 29, 14, 24, 37, 566, DateTimeKind.Utc).AddTicks(3353));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 29, 14, 24, 37, 566, DateTimeKind.Utc).AddTicks(3355));
        }
    }
}
