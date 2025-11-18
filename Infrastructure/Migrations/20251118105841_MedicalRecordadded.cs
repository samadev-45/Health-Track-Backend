using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MedicalRecordadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecord_FileStorage_FileStorageId",
                table: "MedicalRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecord_Users_UserId",
                table: "MedicalRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalRecord",
                table: "MedicalRecord");

            migrationBuilder.RenameTable(
                name: "MedicalRecord",
                newName: "MedicalRecords");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecord_UserId",
                table: "MedicalRecords",
                newName: "IX_MedicalRecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecord_RecordDate",
                table: "MedicalRecords",
                newName: "IX_MedicalRecords_RecordDate");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecord_IsDeleted",
                table: "MedicalRecords",
                newName: "IX_MedicalRecords_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecord_FileStorageId",
                table: "MedicalRecords",
                newName: "IX_MedicalRecords_FileStorageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalRecords",
                table: "MedicalRecords",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_FileStorage_FileStorageId",
                table: "MedicalRecords",
                column: "FileStorageId",
                principalTable: "FileStorage",
                principalColumn: "FileStorageId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Users_UserId",
                table: "MedicalRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_FileStorage_FileStorageId",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Users_UserId",
                table: "MedicalRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalRecords",
                table: "MedicalRecords");

            migrationBuilder.RenameTable(
                name: "MedicalRecords",
                newName: "MedicalRecord");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecords_UserId",
                table: "MedicalRecord",
                newName: "IX_MedicalRecord_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecords_RecordDate",
                table: "MedicalRecord",
                newName: "IX_MedicalRecord_RecordDate");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecords_IsDeleted",
                table: "MedicalRecord",
                newName: "IX_MedicalRecord_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalRecords_FileStorageId",
                table: "MedicalRecord",
                newName: "IX_MedicalRecord_FileStorageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalRecord",
                table: "MedicalRecord",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecord_FileStorage_FileStorageId",
                table: "MedicalRecord",
                column: "FileStorageId",
                principalTable: "FileStorage",
                principalColumn: "FileStorageId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecord_Users_UserId",
                table: "MedicalRecord",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
