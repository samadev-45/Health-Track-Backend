using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnitAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medication_Users_UserId",
                table: "Medication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medication",
                table: "Medication");

            migrationBuilder.RenameTable(
                name: "Medication",
                newName: "Medications");

            migrationBuilder.RenameIndex(
                name: "IX_Medication_UserId",
                table: "Medications",
                newName: "IX_Medications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Medication_UnitId",
                table: "Medications",
                newName: "IX_Medications_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Medication_StartDate",
                table: "Medications",
                newName: "IX_Medications_StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Medication_IsDeleted",
                table: "Medications",
                newName: "IX_Medications_IsDeleted");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medications",
                table: "Medications",
                column: "MedicationId");

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_Units", x => x.UnitId);
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "UnitId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "Description", "IsDeleted", "ModifiedBy", "ModifiedOn", "UnitName" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1493), null, null, "milligram", false, null, null, "mg" },
                    { 2, null, new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1499), null, null, "gram", false, null, null, "g" },
                    { 3, null, new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1503), null, null, "microgram", false, null, null, "mcg" },
                    { 4, null, new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1507), null, null, "milliliter", false, null, null, "ml" },
                    { 5, null, new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1510), null, null, "liter", false, null, null, "L" },
                    { 6, null, new DateTime(2025, 10, 27, 7, 45, 16, 373, DateTimeKind.Utc).AddTicks(1514), null, null, "international unit", false, null, null, "IU" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_DoseRangeUnitId",
                table: "Medications",
                column: "DoseRangeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitName",
                table: "Units",
                column: "UnitName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Units_DoseRangeUnitId",
                table: "Medications",
                column: "DoseRangeUnitId",
                principalTable: "Units",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Units_UnitId",
                table: "Medications",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Users_UserId",
                table: "Medications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Units_DoseRangeUnitId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Units_UnitId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Users_UserId",
                table: "Medications");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medications",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Medications_DoseRangeUnitId",
                table: "Medications");

            migrationBuilder.RenameTable(
                name: "Medications",
                newName: "Medication");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_UserId",
                table: "Medication",
                newName: "IX_Medication_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_UnitId",
                table: "Medication",
                newName: "IX_Medication_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_StartDate",
                table: "Medication",
                newName: "IX_Medication_StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_IsDeleted",
                table: "Medication",
                newName: "IX_Medication_IsDeleted");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medication",
                table: "Medication",
                column: "MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medication_Users_UserId",
                table: "Medication",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
