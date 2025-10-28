using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePasswordSalt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2139));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2146));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2151));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2154));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2158));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2162));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 4, 7, 20, 88, DateTimeKind.Utc).AddTicks(6518));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 4, 7, 20, 88, DateTimeKind.Utc).AddTicks(6524));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 4, 7, 20, 88, DateTimeKind.Utc).AddTicks(6527));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 4, 7, 20, 88, DateTimeKind.Utc).AddTicks(6530));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 4, 7, 20, 88, DateTimeKind.Utc).AddTicks(6533));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 28, 4, 7, 20, 88, DateTimeKind.Utc).AddTicks(6536));
        }
    }
}
