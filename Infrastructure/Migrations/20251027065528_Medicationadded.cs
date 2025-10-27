using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Medicationadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medication",
                columns: table => new
                {
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DosageValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    DoseRangeLow = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    DoseRangeHigh = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    DoseRangeUnitId = table.Column<int>(type: "int", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Medication", x => x.MedicationId);
                    table.ForeignKey(
                        name: "FK_Medication_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medication_IsDeleted",
                table: "Medication",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_StartDate",
                table: "Medication",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_UnitId",
                table: "Medication",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Medication_UserId",
                table: "Medication",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medication");
        }
    }
}
