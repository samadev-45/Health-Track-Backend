using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SharableLinksCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShareableLinks",
                columns: table => new
                {
                    ShareableLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncludeMedicalRecords = table.Column<bool>(type: "bit", nullable: false),
                    IncludeAppointments = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    LastAccessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareableLinks", x => x.ShareableLinkId);
                    table.ForeignKey(
                        name: "FK_ShareableLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 16, 0, 979, DateTimeKind.Utc).AddTicks(2929));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 16, 0, 979, DateTimeKind.Utc).AddTicks(2937));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 16, 0, 979, DateTimeKind.Utc).AddTicks(2942));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 16, 0, 979, DateTimeKind.Utc).AddTicks(2946));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 16, 0, 979, DateTimeKind.Utc).AddTicks(2950));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 16, 0, 979, DateTimeKind.Utc).AddTicks(2955));

            migrationBuilder.CreateIndex(
                name: "IX_ShareableLinks_ExpiresAt",
                table: "ShareableLinks",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ShareableLinks_IsActive",
                table: "ShareableLinks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShareableLinks_IsDeleted",
                table: "ShareableLinks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ShareableLinks_Token",
                table: "ShareableLinks",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShareableLinks_UserId",
                table: "ShareableLinks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShareableLinks");

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 5, 43, 9, DateTimeKind.Utc).AddTicks(259));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 5, 43, 9, DateTimeKind.Utc).AddTicks(265));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 5, 43, 9, DateTimeKind.Utc).AddTicks(269));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 5, 43, 9, DateTimeKind.Utc).AddTicks(286));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 5, 43, 9, DateTimeKind.Utc).AddTicks(291));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 9, 5, 43, 9, DateTimeKind.Utc).AddTicks(295));
        }
    }
}
