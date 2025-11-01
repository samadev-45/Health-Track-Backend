using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    public partial class HardenOtpForEmailLogins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Add hash column
            migrationBuilder.AddColumn<string>(
                name: "OtpCodeHash",
                table: "OtpVerifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            // 2) Create composite index to speed verification queries
            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_User_Purpose_Expiry_Used",
                table: "OtpVerifications",
                columns: new[] { "UserId", "Purpose", "Expiry", "Used" });

            // 3) Optional: invalidate all existing active OTPs to avoid mixed formats
            // migrationBuilder.Sql("UPDATE [OtpVerifications] SET [Used] = 1 WHERE [Used] = 0;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_User_Purpose_Expiry_Used",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "OtpCodeHash",
                table: "OtpVerifications");
        }
    }
}
