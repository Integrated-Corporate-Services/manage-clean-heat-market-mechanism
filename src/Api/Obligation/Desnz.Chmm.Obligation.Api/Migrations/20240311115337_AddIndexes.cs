using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Obligation.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UNIQUE_TRANSACTION",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfTransaction",
                table: "Transactions",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Transactions",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Transactions",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId_OrganisationId_IsExcluded_DateOfT~",
                table: "Transactions",
                columns: new[] { "SchemeYearId", "OrganisationId", "IsExcluded", "DateOfTransaction" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId_OrganisationId_TransactionType",
                table: "Transactions",
                columns: new[] { "SchemeYearId", "OrganisationId", "TransactionType" },
                unique: true,
                filter: "\"TransactionType\" = 'CarryForward' OR \"TransactionType\" = 'BroughtForward'");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId_OrganisationId_TransactionType_Da~",
                table: "Transactions",
                columns: new[] { "SchemeYearId", "OrganisationId", "TransactionType", "DateOfTransaction" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId_SchemeYearQuarterId_OrganisationI~",
                table: "Transactions",
                columns: new[] { "SchemeYearId", "SchemeYearQuarterId", "OrganisationId", "TransactionType", "DateOfTransaction" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SchemeYearId_TransactionType_DateOfTransaction",
                table: "Transactions",
                columns: new[] { "SchemeYearId", "TransactionType", "DateOfTransaction" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId_OrganisationId_IsExcluded_DateOfT~",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId_OrganisationId_TransactionType",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId_OrganisationId_TransactionType_Da~",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId_SchemeYearQuarterId_OrganisationI~",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SchemeYearId_TransactionType_DateOfTransaction",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfTransaction",
                table: "Transactions",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Transactions",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Transactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.CreateIndex(
                name: "IX_UNIQUE_TRANSACTION",
                table: "Transactions",
                columns: new[] { "OrganisationId", "TransactionType", "SchemeYearId" },
                unique: true,
                filter: "\"TransactionType\" = 'CarryForward' OR \"TransactionType\" = 'BroughtForward'");
        }
    }
}
