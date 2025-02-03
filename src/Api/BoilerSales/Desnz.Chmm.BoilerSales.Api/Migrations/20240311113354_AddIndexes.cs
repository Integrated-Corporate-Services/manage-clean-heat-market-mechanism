using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.BoilerSales.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyBoilerSales_SchemeYearId_OrganisationId",
                table: "QuarterlyBoilerSales",
                columns: new[] { "SchemeYearId", "OrganisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyBoilerSales_SchemeYearQuarterId_OrganisationId",
                table: "QuarterlyBoilerSales",
                columns: new[] { "SchemeYearQuarterId", "OrganisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualBoilerSales_OrganisationId_SchemeYearId",
                table: "AnnualBoilerSales",
                columns: new[] { "OrganisationId", "SchemeYearId" });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualBoilerSales_SchemeYearId",
                table: "AnnualBoilerSales",
                column: "SchemeYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuarterlyBoilerSales_SchemeYearId_OrganisationId",
                table: "QuarterlyBoilerSales");

            migrationBuilder.DropIndex(
                name: "IX_QuarterlyBoilerSales_SchemeYearQuarterId_OrganisationId",
                table: "QuarterlyBoilerSales");

            migrationBuilder.DropIndex(
                name: "IX_AnnualBoilerSales_OrganisationId_SchemeYearId",
                table: "AnnualBoilerSales");

            migrationBuilder.DropIndex(
                name: "IX_AnnualBoilerSales_SchemeYearId",
                table: "AnnualBoilerSales");
        }
    }
}
