using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.CreditLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId",
                table: "Transactions",
                column: "SchemeYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId_TransactionType",
                table: "Transactions",
                columns: new[] { "SchemeYearId", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionEntries_OrganisationId",
                table: "TransactionEntries",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationCredits_SchemeYearId_LicenceHolderId",
                table: "InstallationCredits",
                columns: new[] { "SchemeYearId", "LicenceHolderId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId_TransactionType",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TransactionEntries_OrganisationId",
                table: "TransactionEntries");

            migrationBuilder.DropIndex(
                name: "IX_InstallationCredits_SchemeYearId_LicenceHolderId",
                table: "InstallationCredits");
        }
    }
}
