using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FileStorageCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Users_DoctorId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Users_PatientId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistory_Appointment_AppointmentId",
                table: "AppointmentHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistory_Users_ChangedByUserId",
                table: "AppointmentHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentHistory",
                table: "AppointmentHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointment",
                table: "Appointment");

            migrationBuilder.RenameTable(
                name: "AppointmentHistory",
                newName: "AppointmentHistories");

            migrationBuilder.RenameTable(
                name: "Appointment",
                newName: "Appointments");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentHistory_IsDeleted",
                table: "AppointmentHistories",
                newName: "IX_AppointmentHistories_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentHistory_ChangedByUserId",
                table: "AppointmentHistories",
                newName: "IX_AppointmentHistories_ChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentHistory_AppointmentId",
                table: "AppointmentHistories",
                newName: "IX_AppointmentHistories_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_Status",
                table: "Appointments",
                newName: "IX_Appointments_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_PatientId",
                table: "Appointments",
                newName: "IX_Appointments_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_IsDeleted",
                table: "Appointments",
                newName: "IX_Appointments_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_DoctorId",
                table: "Appointments",
                newName: "IX_Appointments_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_AppointmentDate",
                table: "Appointments",
                newName: "IX_Appointments_AppointmentDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentHistories",
                table: "AppointmentHistories",
                column: "HistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "AppointmentId");

            migrationBuilder.CreateTable(
                name: "FileStorages",
                columns: table => new
                {
                    FileStorageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedByUserId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_FileStorages", x => x.FileStorageId);
                    table.ForeignKey(
                        name: "FK_FileStorages_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorages_IsDeleted",
                table: "FileStorages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorages_UploadedByUserId",
                table: "FileStorages",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistories_Appointments_AppointmentId",
                table: "AppointmentHistories",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistories_Users_ChangedByUserId",
                table: "AppointmentHistories",
                column: "ChangedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistories_Appointments_AppointmentId",
                table: "AppointmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentHistories_Users_ChangedByUserId",
                table: "AppointmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_PatientId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "FileStorages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentHistories",
                table: "AppointmentHistories");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "Appointment");

            migrationBuilder.RenameTable(
                name: "AppointmentHistories",
                newName: "AppointmentHistory");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_Status",
                table: "Appointment",
                newName: "IX_Appointment_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointment",
                newName: "IX_Appointment_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_IsDeleted",
                table: "Appointment",
                newName: "IX_Appointment_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointment",
                newName: "IX_Appointment_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_AppointmentDate",
                table: "Appointment",
                newName: "IX_Appointment_AppointmentDate");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentHistories_IsDeleted",
                table: "AppointmentHistory",
                newName: "IX_AppointmentHistory_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentHistories_ChangedByUserId",
                table: "AppointmentHistory",
                newName: "IX_AppointmentHistory_ChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentHistories_AppointmentId",
                table: "AppointmentHistory",
                newName: "IX_AppointmentHistory_AppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointment",
                table: "Appointment",
                column: "AppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentHistory",
                table: "AppointmentHistory",
                column: "HistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Users_DoctorId",
                table: "Appointment",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Users_PatientId",
                table: "Appointment",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistory_Appointment_AppointmentId",
                table: "AppointmentHistory",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentHistory_Users_ChangedByUserId",
                table: "AppointmentHistory",
                column: "ChangedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
