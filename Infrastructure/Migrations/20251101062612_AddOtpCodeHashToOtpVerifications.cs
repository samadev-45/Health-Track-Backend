using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpCodeHashToOtpVerifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_UserId",
                table: "OtpVerifications");

            migrationBuilder.AlterColumn<int>(
                name: "Attempts",
                table: "OtpVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "OtpCodeHash",
                table: "OtpVerifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_UserId_Purpose_Expiry_Used",
                table: "OtpVerifications",
                columns: new[] { "UserId", "Purpose", "Expiry", "Used" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_UserId_Purpose_Expiry_Used",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "OtpCodeHash",
                table: "OtpVerifications");

            migrationBuilder.AlterColumn<int>(
                name: "Attempts",
                table: "OtpVerifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_UserId",
                table: "OtpVerifications",
                column: "UserId");
        }
    }
}
