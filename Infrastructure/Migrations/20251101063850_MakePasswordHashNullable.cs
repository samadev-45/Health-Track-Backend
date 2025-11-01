using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    public partial class MakePasswordHashNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make Users.PasswordHash nullable (required for OTP-only caretakers)
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // If OtpCodeHash is still missing, add it now (safe if not present earlier)
            migrationBuilder.AddColumn<string>(
                name: "OtpCodeHash",
                table: "OtpVerifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            // Ensure the composite index exists for fast OTP verification lookups
            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_User_Purpose_Expiry_Used",
                table: "OtpVerifications",
                columns: new[] { "UserId", "Purpose", "Expiry", "Used" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop index if it was created in Up
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_User_Purpose_Expiry_Used",
                table: "OtpVerifications");

            // Drop OtpCodeHash only if it exists from this migration
            migrationBuilder.DropColumn(
                name: "OtpCodeHash",
                table: "OtpVerifications");

            // Revert Users.PasswordHash to non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
