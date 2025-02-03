using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.McsSynchronisation.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsSystemSelectedAsMCSTechnologyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerName",
                table: "HeatPumpProducts",
                type: "varchar(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemSelectedAsMCSTechnology",
                table: "HeatPumpInstallations",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystemSelectedAsMCSTechnology",
                table: "HeatPumpInstallations");

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerName",
                table: "HeatPumpProducts",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)");
        }
    }
}
