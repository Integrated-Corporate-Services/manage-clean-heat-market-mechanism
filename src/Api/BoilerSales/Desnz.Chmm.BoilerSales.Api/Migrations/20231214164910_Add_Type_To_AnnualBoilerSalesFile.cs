using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.BoilerSales.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_Type_To_AnnualBoilerSalesFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileKey",
                table: "AnnualBoilerSalesFiles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AnnualBoilerSalesFiles",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "AnnualBoilerSalesFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FileKey",
                table: "AnnualBoilerSalesFiles",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");
        }
    }
}
