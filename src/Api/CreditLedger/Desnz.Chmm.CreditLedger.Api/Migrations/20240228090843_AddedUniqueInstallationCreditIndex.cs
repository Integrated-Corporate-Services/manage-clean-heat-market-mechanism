using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.CreditLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueInstallationCreditIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries");

            migrationBuilder.CreateIndex(
                name: "IX_UNIQUE_INSTALLATION_CREDIT",
                table: "InstallationCredits",
                columns: new[] { "LicenceHolderId", "HeatPumpInstallationId", "SchemeYearId", "IsHybrid" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries");

            migrationBuilder.DropIndex(
                name: "IX_UNIQUE_INSTALLATION_CREDIT",
                table: "InstallationCredits");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
