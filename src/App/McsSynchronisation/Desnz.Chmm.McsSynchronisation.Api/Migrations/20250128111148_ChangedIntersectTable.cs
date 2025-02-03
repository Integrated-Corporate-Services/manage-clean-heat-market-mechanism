using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.McsSynchronisation.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangedIntersectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HeatPumpInstallationProducts",
                table: "HeatPumpInstallationProducts");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "HeatPumpInstallationProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HeatPumpInstallationProducts",
                table: "HeatPumpInstallationProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HeatPumpInstallationProducts_InstallationId",
                table: "HeatPumpInstallationProducts",
                column: "InstallationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HeatPumpInstallationProducts",
                table: "HeatPumpInstallationProducts");

            migrationBuilder.DropIndex(
                name: "IX_HeatPumpInstallationProducts_InstallationId",
                table: "HeatPumpInstallationProducts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "HeatPumpInstallationProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HeatPumpInstallationProducts",
                table: "HeatPumpInstallationProducts",
                columns: new[] { "InstallationId", "ProductId" });
        }
    }
}
