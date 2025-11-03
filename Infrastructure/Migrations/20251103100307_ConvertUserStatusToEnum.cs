using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertUserStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Add a temporary int column
            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 2️⃣ Copy existing string values into enum int values
            migrationBuilder.Sql(@"
        UPDATE Users
        SET StatusInt = 
            CASE Status
                WHEN 'Pending' THEN 0
                WHEN 'Approved' THEN 1
                WHEN 'Rejected' THEN 2
                WHEN 'Disabled' THEN 3
                ELSE 0
            END
    ");

            // 3️⃣ Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            // 4️⃣ Rename new column to Status
            migrationBuilder.RenameColumn(
                name: "StatusInt",
                table: "Users",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert changes if needed
            migrationBuilder.AddColumn<string>(
                name: "StatusText",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
        UPDATE Users
        SET StatusText =
            CASE Status
                WHEN 0 THEN 'Pending'
                WHEN 1 THEN 'Approved'
                WHEN 2 THEN 'Rejected'
                WHEN 3 THEN 'Disabled'
                ELSE 'Pending'
            END
    ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "StatusText",
                table: "Users",
                newName: "Status");
        }
    }
}
