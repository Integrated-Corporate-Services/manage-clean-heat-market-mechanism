using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    /// <inheritdoc />
    public partial class BoilerSalesSubmissionEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings");

            migrationBuilder.DropForeignKey(
                name: "FK_ObligationCalculations_SchemeYears_SchemeYearId",
                table: "ObligationCalculations");

            migrationBuilder.AddColumn<DateOnly>(
                name: "BoilerSalesSubmissionEndDate",
                table: "SchemeYears",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId",
                principalTable: "SchemeYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationCalculations_SchemeYears_SchemeYearId",
                table: "ObligationCalculations",
                column: "SchemeYearId",
                principalTable: "SchemeYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings");

            migrationBuilder.DropForeignKey(
                name: "FK_ObligationCalculations_SchemeYears_SchemeYearId",
                table: "ObligationCalculations");

            migrationBuilder.DropColumn(
                name: "BoilerSalesSubmissionEndDate",
                table: "SchemeYears");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditWeightings_SchemeYears_SchemeYearId",
                table: "CreditWeightings",
                column: "SchemeYearId",
                principalTable: "SchemeYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObligationCalculations_SchemeYears_SchemeYearId",
                table: "ObligationCalculations",
                column: "SchemeYearId",
                principalTable: "SchemeYears",
                principalColumn: "Id");
        }
    }
}
