using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class Organisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "ChmmUsers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleOfficerOrganisationName",
                table: "ChmmUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneNumber",
                table: "ChmmUsers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ChmmUsers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserOrganisationId",
                table: "ChmmUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    CompaniesHouseNumber = table.Column<string>(type: "varchar(100)", nullable: true),
                    IsGroupRegistration = table.Column<bool>(type: "boolean", nullable: false),
                    IsFossilFuelBoilerSeller = table.Column<bool>(type: "boolean", nullable: false),
                    HeatPumpBrands = table.Column<string[]>(type: "varchar(100)[]", nullable: true),
                    ContactName = table.Column<string>(type: "varchar(100)", nullable: true),
                    ContactEmail = table.Column<string>(type: "varchar(100)", nullable: true),
                    ContactTelephoneNumber = table.Column<string>(type: "varchar(100)", nullable: true),
                    Status = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationAddresss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", nullable: false),
                    AddressOrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineOne = table.Column<string>(type: "varchar(100)", nullable: false),
                    LineTwo = table.Column<string>(type: "varchar(100)", nullable: true),
                    City = table.Column<string>(type: "varchar(100)", nullable: false),
                    County = table.Column<string>(type: "varchar(100)", nullable: true),
                    PostCode = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAddresss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationAddresss_Organisations_AddressOrganisationId",
                        column: x => x.AddressOrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChmmUsers_Email",
                table: "ChmmUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChmmUsers_UserOrganisationId",
                table: "ChmmUsers",
                column: "UserOrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChmmRoles_Name",
                table: "ChmmRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationAddresss_AddressOrganisationId",
                table: "OrganisationAddresss",
                column: "AddressOrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChmmUsers_Organisations_UserOrganisationId",
                table: "ChmmUsers",
                column: "UserOrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChmmUsers_Organisations_UserOrganisationId",
                table: "ChmmUsers");

            migrationBuilder.DropTable(
                name: "OrganisationAddresss");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_ChmmUsers_Email",
                table: "ChmmUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChmmUsers_UserOrganisationId",
                table: "ChmmUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChmmRoles_Name",
                table: "ChmmRoles");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "ChmmUsers");

            migrationBuilder.DropColumn(
                name: "ResponsibleOfficerOrganisationName",
                table: "ChmmUsers");

            migrationBuilder.DropColumn(
                name: "TelephoneNumber",
                table: "ChmmUsers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChmmUsers");

            migrationBuilder.DropColumn(
                name: "UserOrganisationId",
                table: "ChmmUsers");
        }
    }
}
