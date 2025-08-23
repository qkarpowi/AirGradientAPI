using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirGradientAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenamingTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "SensorData",
                newName: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "SensorData",
                newName: "timestamp");
        }
    }
}
