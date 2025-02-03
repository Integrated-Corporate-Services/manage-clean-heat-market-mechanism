using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class LicenceHolder_OrganisationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "LicenseHolders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenseHolders_OrganisationId",
                table: "LicenseHolders",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseHolders_Organisations_OrganisationId",
                table: "LicenseHolders",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseHolders_Organisations_OrganisationId",
                table: "LicenseHolders");

            migrationBuilder.DropIndex(
                name: "IX_LicenseHolders_OrganisationId",
                table: "LicenseHolders");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "LicenseHolders");
        }
    }
}
