using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedObligationCalculations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings");

            migrationBuilder.DropIndex(
                name: "IX_CreditWeightings_SchemeYearId",
                table: "CreditWeightings");

            migrationBuilder.DropColumn(
                name: "GasBoilerSalesThreshold",
                table: "SchemeYears");

            migrationBuilder.DropColumn(
                name: "OilBoilerSalesThreshold",
                table: "SchemeYears");

            migrationBuilder.DropColumn(
                name: "TargetRate",
                table: "SchemeYears");

            migrationBuilder.CreateTable(
                name: "ObligationCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemeYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    PercentageCap = table.Column<decimal>(type: "decimal", nullable: false),
                    TargetMultiplier = table.Column<decimal>(type: "decimal", nullable: false),
                    GasCreditsCap = table.Column<int>(type: "int", nullable: false),
                    OilCreditsCap = table.Column<int>(type: "int", nullable: false),
                    GasBoilerSalesThreshold = table.Column<int>(type: "int", nullable: false),
                    OilBoilerSalesThreshold = table.Column<int>(type: "int", nullable: false),
                    TargetRate = table.Column<decimal>(type: "decimal", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationCalculations_SchemeYears_SchemeYearId",
                        column: x => x.SchemeYearId,
                        principalTable: "SchemeYears",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditWeightings_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObligationCalculations_SchemeYearId",
                table: "ObligationCalculations",
                column: "SchemeYearId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId",
                principalTable: "SchemeYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings");

            migrationBuilder.DropTable(
                name: "ObligationCalculations");

            migrationBuilder.DropIndex(
                name: "IX_CreditWeightings_SchemeYearId",
                table: "CreditWeightings");

            migrationBuilder.AddColumn<int>(
                name: "GasBoilerSalesThreshold",
                table: "SchemeYears",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OilBoilerSalesThreshold",
                table: "SchemeYears",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TargetRate",
                table: "SchemeYears",
                type: "decimal",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CreditWeightings_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId",
                principalTable: "SchemeYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
