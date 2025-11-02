using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HealthMatrix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthMetrics",
                columns: table => new
                {
                    HealthMetricId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MetricCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAbnormal = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_HealthMetrics", x => x.HealthMetricId);
                    table.ForeignKey(
                        name: "FK_HealthMetrics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_IsDeleted",
                table: "HealthMetrics",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_MeasuredAt",
                table: "HealthMetrics",
                column: "MeasuredAt");

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_MetricCode",
                table: "HealthMetrics",
                column: "MetricCode");

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_UserId",
                table: "HealthMetrics",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthMetrics");
        }
    }
}
