using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.CreditLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfTransaction",
                table: "CreditTransfers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CreditTransfers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfTransaction",
                table: "Transactions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "InitiatedBy",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfTransaction",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InitiatedBy",
                table: "Transactions");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfTransaction",
                table: "CreditTransfers",
                type: "timestamptz",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CreditTransfers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
