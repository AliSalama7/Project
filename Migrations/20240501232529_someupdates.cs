using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class someupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnergyUsageData");

            migrationBuilder.DropTable(
                name: "LightData");

            migrationBuilder.DropTable(
                name: "MotionData");

            migrationBuilder.DropTable(
                name: "WaterLevelData");

            migrationBuilder.CreateTable(
                name: "TempAndHumidity",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SensorValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempAndHumidity", x => x.SensorID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempAndHumidity");

            migrationBuilder.CreateTable(
                name: "EnergyUsageData",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnergyUsageValue = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyUsageData", x => x.SensorID);
                });

            migrationBuilder.CreateTable(
                name: "LightData",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LightValue = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightData", x => x.SensorID);
                });

            migrationBuilder.CreateTable(
                name: "MotionData",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MotionDetected = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionData", x => x.SensorID);
                });

            migrationBuilder.CreateTable(
                name: "WaterLevelData",
                columns: table => new
                {
                    SensorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WaterLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterLevelData", x => x.SensorID);
                });
        }
    }
}
