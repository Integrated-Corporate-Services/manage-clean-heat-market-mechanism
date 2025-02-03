using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    /// <inheritdoc />
    public partial class PreviousSchemeYearId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PreviousSchemeYearId",
                table: "SchemeYears",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousSchemeYearId",
                table: "SchemeYears");
        }
    }
}
