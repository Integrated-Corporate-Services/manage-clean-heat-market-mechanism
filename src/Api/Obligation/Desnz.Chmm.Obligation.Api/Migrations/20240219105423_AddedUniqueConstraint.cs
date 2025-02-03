using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Obligation.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UNIQUE_TRANSACTION",
                table: "Transactions",
                columns: new[] { "OrganisationId", "TransactionType", "SchemeYearId" },
                unique: true,
                filter: "\"TransactionType\" = 'CarryForward' OR \"TransactionType\" = 'BroughtForward'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UNIQUE_TRANSACTION",
                table: "Transactions");
        }
    }
}
