using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Obligation.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTransactionManufacturerId_With_OrganisationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Transactions");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
