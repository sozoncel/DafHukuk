using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DafHukuk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSolutionPartnersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 29, 13, 42, 27, 458, DateTimeKind.Utc).AddTicks(2986));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 29, 13, 42, 27, 458, DateTimeKind.Utc).AddTicks(2988));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 12, 29, 13, 42, 27, 458, DateTimeKind.Utc).AddTicks(2990));
        }
    }
}
