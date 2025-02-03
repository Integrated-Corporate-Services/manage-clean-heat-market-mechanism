using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueConstraintOnManufacturerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenceHolderLinks_Organisations_OrganisationId",
                table: "LicenceHolderLinks");

            migrationBuilder.CreateIndex(
                name: "IX_UNIQUE_LICENCE_HOLDER",
                table: "LicenceHolders",
                column: "McsManufacturerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LicenceHolderLinks_Organisations_OrganisationId",
                table: "LicenceHolderLinks",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenceHolderLinks_Organisations_OrganisationId",
                table: "LicenceHolderLinks");

            migrationBuilder.DropIndex(
                name: "IX_UNIQUE_LICENCE_HOLDER",
                table: "LicenceHolders");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenceHolderLinks_Organisations_OrganisationId",
                table: "LicenceHolderLinks",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
