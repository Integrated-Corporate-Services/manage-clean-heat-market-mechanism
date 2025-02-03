using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.CreditLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class LicenceHolder_Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseHolderId",
                table: "InstallationCredits",
                newName: "LicenceHolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenceHolderId",
                table: "InstallationCredits",
                newName: "LicenseHolderId");
        }
    }
}
