using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MediactionReminderAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicationReminders",
                columns: table => new
                {
                    ReminderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: false),
                    RemindAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_MedicationReminders", x => x.ReminderId);
                    table.ForeignKey(
                        name: "FK_MedicationReminders_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReminders_IsDeleted",
                table: "MedicationReminders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReminders_IsSent",
                table: "MedicationReminders",
                column: "IsSent");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReminders_MedicationId",
                table: "MedicationReminders",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReminders_RemindAt",
                table: "MedicationReminders",
                column: "RemindAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationReminders");

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1493));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1499));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1503));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1507));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1510));

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "UnitId",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1514));
        }
    }
}
