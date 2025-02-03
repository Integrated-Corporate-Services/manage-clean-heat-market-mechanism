using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.SystemAudit.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuditType",
                table: "AuditItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "AuditItems",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditType",
                table: "AuditItems");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "AuditItems");
        }
    }
}
