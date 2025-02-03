using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class OrganisationComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChmmUsers_Organisations_UserOrganisationId",
                table: "ChmmUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationAddresss_Organisations_AddressOrganisationId",
                table: "OrganisationAddresss");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChmmUsers");

            migrationBuilder.RenameColumn(
                name: "AddressOrganisationId",
                table: "OrganisationAddresss",
                newName: "OrganisationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganisationAddresss_AddressOrganisationId",
                table: "OrganisationAddresss",
                newName: "IX_OrganisationAddresss_OrganisationId");

            migrationBuilder.RenameColumn(
                name: "UserOrganisationId",
                table: "ChmmUsers",
                newName: "OrganisationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChmmUsers_UserOrganisationId",
                table: "ChmmUsers",
                newName: "IX_ChmmUsers_OrganisationId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantId",
                table: "Organisations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ResponsibleOfficerId",
                table: "Organisations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "OrganisationComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "varchar(100)", nullable: false),
                    ChmmUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationComments_ChmmUsers_ChmmUserId",
                        column: x => x.ChmmUserId,
                        principalTable: "ChmmUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationComments_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_Name",
                table: "Organisations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ChmmUsers_Name",
                table: "ChmmUsers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationComments_ChmmUserId",
                table: "OrganisationComments",
                column: "ChmmUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationComments_OrganisationId",
                table: "OrganisationComments",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChmmUsers_Organisations_OrganisationId",
                table: "ChmmUsers",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationAddresss_Organisations_OrganisationId",
                table: "OrganisationAddresss",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChmmUsers_Organisations_OrganisationId",
                table: "ChmmUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganisationAddresss_Organisations_OrganisationId",
                table: "OrganisationAddresss");

            migrationBuilder.DropTable(
                name: "OrganisationComments");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_Name",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_ChmmUsers_Name",
                table: "ChmmUsers");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "ResponsibleOfficerId",
                table: "Organisations");

            migrationBuilder.RenameColumn(
                name: "OrganisationId",
                table: "OrganisationAddresss",
                newName: "AddressOrganisationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganisationAddresss_OrganisationId",
                table: "OrganisationAddresss",
                newName: "IX_OrganisationAddresss_AddressOrganisationId");

            migrationBuilder.RenameColumn(
                name: "OrganisationId",
                table: "ChmmUsers",
                newName: "UserOrganisationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChmmUsers_OrganisationId",
                table: "ChmmUsers",
                newName: "IX_ChmmUsers_UserOrganisationId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ChmmUsers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChmmUsers_Organisations_UserOrganisationId",
                table: "ChmmUsers",
                column: "UserOrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganisationAddresss_Organisations_AddressOrganisationId",
                table: "OrganisationAddresss",
                column: "AddressOrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
