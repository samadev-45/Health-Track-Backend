using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MedicalRecordAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalRecord",
                columns: table => new
                {
                    RecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RecordTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoctorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hospital = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileStorageId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_MedicalRecord", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_FileStorages_FileStorageId",
                        column: x => x.FileStorageId,
                        principalTable: "FileStorages",
                        principalColumn: "FileStorageId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_FileStorageId",
                table: "MedicalRecord",
                column: "FileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_IsDeleted",
                table: "MedicalRecord",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_RecordDate",
                table: "MedicalRecord",
                column: "RecordDate");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_UserId",
                table: "MedicalRecord",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalRecord");
        }
    }
}
