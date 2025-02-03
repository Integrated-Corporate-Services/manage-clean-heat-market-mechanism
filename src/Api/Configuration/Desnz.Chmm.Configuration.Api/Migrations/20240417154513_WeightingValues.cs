using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    /// <inheritdoc />
    public partial class WeightingValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "AlternativeSystemFuelTypeWeightings");

            migrationBuilder.AddColumn<Guid>(
                name: "AlternativeSystemFuelTypeWeightingValueId",
                table: "AlternativeSystemFuelTypeWeightings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AlternativeSystemFuelTypeWeightingValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "decimal", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativeSystemFuelTypeWeightingValues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternativeSystemFuelTypeWeightings_AlternativeSystemFuelTy~",
                table: "AlternativeSystemFuelTypeWeightings",
                column: "AlternativeSystemFuelTypeWeightingValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlternativeSystemFuelTypeWeightings_AlternativeSystemFuelTy~",
                table: "AlternativeSystemFuelTypeWeightings",
                column: "AlternativeSystemFuelTypeWeightingValueId",
                principalTable: "AlternativeSystemFuelTypeWeightingValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlternativeSystemFuelTypeWeightings_AlternativeSystemFuelTy~",
                table: "AlternativeSystemFuelTypeWeightings");

            migrationBuilder.DropTable(
                name: "AlternativeSystemFuelTypeWeightingValues");

            migrationBuilder.DropIndex(
                name: "IX_AlternativeSystemFuelTypeWeightings_AlternativeSystemFuelTy~",
                table: "AlternativeSystemFuelTypeWeightings");

            migrationBuilder.DropColumn(
                name: "AlternativeSystemFuelTypeWeightingValueId",
                table: "AlternativeSystemFuelTypeWeightings");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "AlternativeSystemFuelTypeWeightings",
                type: "decimal",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
