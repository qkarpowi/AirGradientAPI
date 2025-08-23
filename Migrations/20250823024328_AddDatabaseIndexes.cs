using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirGradientAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChipId",
                table: "SensorData",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_CO2",
                table: "SensorData",
                column: "Rco2");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_Timestamp",
                table: "SensorData",
                column: "timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorData_CO2",
                table: "SensorData");

            migrationBuilder.DropIndex(
                name: "IX_SensorData_Timestamp",
                table: "SensorData");

            migrationBuilder.AlterColumn<string>(
                name: "ChipId",
                table: "SensorData",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
