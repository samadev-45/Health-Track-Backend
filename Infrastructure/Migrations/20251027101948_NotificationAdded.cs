using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
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
                value: new DateTime(2025, 10, 27, 10, 19, 47, 221, DateTimeKind.Utc).AddTicks(6603));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 10, 19, 47, 221, DateTimeKind.Utc).AddTicks(6608));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 10, 19, 47, 221, DateTimeKind.Utc).AddTicks(6611));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 10, 19, 47, 221, DateTimeKind.Utc).AddTicks(6614));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 10, 19, 47, 221, DateTimeKind.Utc).AddTicks(6616));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 10, 19, 47, 221, DateTimeKind.Utc).AddTicks(6619));

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsDeleted",
                table: "Notifications",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Priority",
                table: "Notifications",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

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
        }
    }
}
