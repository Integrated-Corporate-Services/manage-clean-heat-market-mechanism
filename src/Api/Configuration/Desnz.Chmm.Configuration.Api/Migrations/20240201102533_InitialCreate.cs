using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "SchemeYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TradingWindowStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TradingWindowEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreditGenerationWindowStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreditGenerationWindowEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GasBoilerSalesThreshold = table.Column<int>(type: "int", nullable: false),
                    OilBoilerSalesThreshold = table.Column<int>(type: "int", nullable: false),
                    TargetRate = table.Column<decimal>(type: "decimal", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditWeightings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalCapacity = table.Column<int>(type: "int", nullable: false),
                    SchemeYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditWeightings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                        column: x => x.SchemeYearId,
                        principalTable: "SchemeYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemeYearQuarters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SchemeYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemeYearQuarters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeYearQuarters_SchemeYears_SchemeYearId",
                        column: x => x.SchemeYearId,
                        principalTable: "SchemeYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlternativeSystemFuelTypeWeightings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "decimal", nullable: false),
                    Code = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreditWeightingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativeSystemFuelTypeWeightings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlternativeSystemFuelTypeWeightings_CreditWeightings_Credit~",
                        column: x => x.CreditWeightingId,
                        principalTable: "CreditWeightings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeatPumpTechnologyTypeWeightings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "decimal", nullable: false),
                    Code = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreditWeightingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatPumpTechnologyTypeWeightings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeatPumpTechnologyTypeWeightings_CreditWeightings_CreditWei~",
                        column: x => x.CreditWeightingId,
                        principalTable: "CreditWeightings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternativeSystemFuelTypeWeightings_CreditWeightingId",
                table: "AlternativeSystemFuelTypeWeightings",
                column: "CreditWeightingId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditWeightings_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId");

            migrationBuilder.CreateIndex(
                name: "IX_HeatPumpTechnologyTypeWeightings_CreditWeightingId",
                table: "HeatPumpTechnologyTypeWeightings",
                column: "CreditWeightingId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeYearQuarters_SchemeYearId",
                table: "SchemeYearQuarters",
                column: "SchemeYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternativeSystemFuelTypeWeightings");

            migrationBuilder.DropTable(
                name: "HeatPumpTechnologyTypeWeightings");

            migrationBuilder.DropTable(
                name: "SchemeYearQuarters");

            migrationBuilder.DropTable(
                name: "CreditWeightings");

            migrationBuilder.DropTable(
                name: "SchemeYears");
        }
    }
}
