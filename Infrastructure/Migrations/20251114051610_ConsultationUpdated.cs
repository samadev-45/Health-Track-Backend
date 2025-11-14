using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsultationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HealthValuesJson",
                table: "Consultation");

            migrationBuilder.AddColumn<string>(
                name: "HealthValues",
                table: "Consultation",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HealthValues",
                table: "Consultation");

            migrationBuilder.AddColumn<string>(
                name: "HealthValuesJson",
                table: "Consultation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
