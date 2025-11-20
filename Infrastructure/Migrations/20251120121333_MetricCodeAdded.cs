using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MetricCodeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HealthMetrics_MetricCode",
                table: "HealthMetrics");

            migrationBuilder.DropColumn(
                name: "MetricCode",
                table: "HealthMetrics");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "HealthMetrics");

            migrationBuilder.AddColumn<int>(
                name: "MetricTypeId",
                table: "HealthMetrics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MetricType",
                schema: "master",
                columns: table => new
                {
                    MetricTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetricCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MaxValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    table.PrimaryKey("PK_MetricType", x => x.MetricTypeId);
                });

            migrationBuilder.InsertData(
                schema: "master",
                table: "MetricType",
                columns: new[] { "MetricTypeId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "DisplayName", "IsActive", "IsDeleted", "MaxValue", "MetricCode", "MinValue", "ModifiedBy", "ModifiedOn", "Unit" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Blood Pressure (Systolic)", true, false, 120m, "bp_sys", 80m, null, null, "mmHg" },
                    { 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Blood Pressure (Diastolic)", true, false, 80m, "bp_dia", 60m, null, null, "mmHg" },
                    { 3, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Blood Glucose (Fasting)", true, false, 100m, "glucose_fast", 70m, null, null, "mg/dL" },
                    { 4, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Blood Glucose (Postprandial)", true, false, 140m, "glucose_pp", 70m, null, null, "mg/dL" },
                    { 5, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Oxygen Saturation", true, false, null, "spo2", 95m, null, null, "%" },
                    { 6, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Heart Rate", true, false, 100m, "heart_rate", 60m, null, null, "bpm" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_MetricTypeId",
                table: "HealthMetrics",
                column: "MetricTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricType_MetricCode",
                schema: "master",
                table: "MetricType",
                column: "MetricCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HealthMetrics_MetricType_MetricTypeId",
                table: "HealthMetrics",
                column: "MetricTypeId",
                principalSchema: "master",
                principalTable: "MetricType",
                principalColumn: "MetricTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthMetrics_MetricType_MetricTypeId",
                table: "HealthMetrics");

            migrationBuilder.DropTable(
                name: "MetricType",
                schema: "master");

            migrationBuilder.DropIndex(
                name: "IX_HealthMetrics_MetricTypeId",
                table: "HealthMetrics");

            migrationBuilder.DropColumn(
                name: "MetricTypeId",
                table: "HealthMetrics");

            migrationBuilder.AddColumn<string>(
                name: "MetricCode",
                table: "HealthMetrics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "HealthMetrics",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_MetricCode",
                table: "HealthMetrics",
                column: "MetricCode");
        }
    }
}
