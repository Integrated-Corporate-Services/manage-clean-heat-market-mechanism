using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.CreditLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class CascadeTransactionEntityDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionEntries_Transactions_TransactionId",
                table: "TransactionEntries",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
