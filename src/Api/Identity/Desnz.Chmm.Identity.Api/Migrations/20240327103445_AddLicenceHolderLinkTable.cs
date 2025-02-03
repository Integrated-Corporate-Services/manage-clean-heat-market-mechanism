using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenceHolderLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenceHolders_Organisations_OrganisationId",
                table: "LicenceHolders");

            migrationBuilder.DropIndex(
                name: "IX_LicenceHolders_OrganisationId",
                table: "LicenceHolders");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "LicenceHolders");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "LicenceHolders");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "LicenceHolders");

            migrationBuilder.CreateTable(
                name: "LicenceHolderLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LicenceHolderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenceHolderLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenceHolderLinks_LicenceHolders_LicenceHolderId",
                        column: x => x.LicenceHolderId,
                        principalTable: "LicenceHolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenceHolderLinks_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicenceHolderLinks_LicenceHolderId",
                table: "LicenceHolderLinks",
                column: "LicenceHolderId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenceHolderLinks_OrganisationId",
                table: "LicenceHolderLinks",
                column: "OrganisationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenceHolderLinks");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "LicenceHolders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "LicenceHolders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "LicenceHolders",
                type: "date",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenceHolders_OrganisationId",
                table: "LicenceHolders",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenceHolders_Organisations_OrganisationId",
                table: "LicenceHolders",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");
        }
    }
}
