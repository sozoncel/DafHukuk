
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DafHukuk.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 28, 11, 34, 18, 668, DateTimeKind.Utc).AddTicks(136));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 28, 11, 34, 18, 668, DateTimeKind.Utc).AddTicks(137));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 28, 11, 34, 18, 668, DateTimeKind.Utc).AddTicks(139));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 28, 10, 42, 56, 850, DateTimeKind.Utc).AddTicks(7111));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 28, 10, 42, 56, 850, DateTimeKind.Utc).AddTicks(7113));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 28, 10, 42, 56, 850, DateTimeKind.Utc).AddTicks(7115));
        }
    }
}
