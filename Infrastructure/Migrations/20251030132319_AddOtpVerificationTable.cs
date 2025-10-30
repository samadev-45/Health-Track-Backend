using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpVerificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6);

            migrationBuilder.CreateTable(
                name: "OtpVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Expiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpVerifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_UserId",
                table: "OtpVerifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpVerifications");

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "UnitId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Description", "IsDeleted", "ModifiedBy", "ModifiedOn", "UnitName" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2139), null, null, "milligram", false, null, null, "mg" },
                    { 2, null, new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2146), null, null, "gram", false, null, null, "g" },
                    { 3, null, new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2151), null, null, "microgram", false, null, null, "mcg" },
                    { 4, null, new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2154), null, null, "milliliter", false, null, null, "ml" },
                    { 5, null, new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2158), null, null, "liter", false, null, null, "L" },
                    { 6, null, new DateTime(2025, 10, 28, 5, 38, 0, 818, DateTimeKind.Utc).AddTicks(2162), null, null, "international unit", false, null, null, "IU" }
                });
        }
    }
}
