using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecordTypeMastradded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecordTypeId1",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecordType",
                schema: "master",
                columns: table => new
                {
                    RecordTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_RecordType", x => x.RecordTypeId);
                });

            migrationBuilder.InsertData(
                schema: "master",
                table: "RecordType",
                columns: new[] { "RecordTypeId", "CreatedBy", "CreatedOn", "DeletedBy", "DeletedOn", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, null, "Prescription" },
                    { 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, null, "Lab Report" },
                    { 3, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, null, "X-Ray" },
                    { 4, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, null, "MRI" },
                    { 5, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, null, "Scan" },
                    { 6, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, null, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_RecordTypeId",
                table: "MedicalRecords",
                column: "RecordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_RecordTypeId1",
                table: "MedicalRecords",
                column: "RecordTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_RecordType_RecordTypeId",
                table: "MedicalRecords",
                column: "RecordTypeId",
                principalSchema: "master",
                principalTable: "RecordType",
                principalColumn: "RecordTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_RecordType_RecordTypeId1",
                table: "MedicalRecords",
                column: "RecordTypeId1",
                principalSchema: "master",
                principalTable: "RecordType",
                principalColumn: "RecordTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_RecordType_RecordTypeId",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_RecordType_RecordTypeId1",
                table: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "RecordType",
                schema: "master");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_RecordTypeId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_RecordTypeId1",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "RecordTypeId1",
                table: "MedicalRecords");
        }
    }
}
