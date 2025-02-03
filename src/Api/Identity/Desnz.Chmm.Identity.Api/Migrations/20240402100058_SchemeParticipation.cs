using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class SchemeParticipation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenceHolderLinks_Organisations_OrganisationId",
                table: "LicenceHolderLinks");

            migrationBuilder.AddColumn<string>(
                name: "LegalAddressType",
                table: "Organisations",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.DropColumn(
                name: "LegalAddressType",
                table: "Organisations");

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
