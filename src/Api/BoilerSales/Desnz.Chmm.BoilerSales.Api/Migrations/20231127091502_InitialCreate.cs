using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.BoilerSales.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "AnnualBoilerSales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemeYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gas = table.Column<int>(type: "integer", nullable: false),
                    Oil = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualBoilerSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuarterlyBoilerSales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemeYearQuarterId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Gas = table.Column<int>(type: "integer", nullable: false),
                    Oil = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterlyBoilerSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualBoilerSalesChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnualBoilerSalesId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldGas = table.Column<int>(type: "integer", nullable: true),
                    OldOil = table.Column<int>(type: "integer", nullable: true),
                    OldStatus = table.Column<string>(type: "varchar(50)", nullable: true),
                    NewGas = table.Column<int>(type: "integer", nullable: false),
                    NewOil = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<string>(type: "varchar(50)", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualBoilerSalesChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualBoilerSalesChanges_AnnualBoilerSales_AnnualBoilerSale~",
                        column: x => x.AnnualBoilerSalesId,
                        principalTable: "AnnualBoilerSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnualBoilerSalesFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnualBoilerSalesId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileKey = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualBoilerSalesFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualBoilerSalesFiles_AnnualBoilerSales_AnnualBoilerSalesId",
                        column: x => x.AnnualBoilerSalesId,
                        principalTable: "AnnualBoilerSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuarterlyBoilerSalesChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuarterlyBoilerSalesId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldGas = table.Column<int>(type: "integer", nullable: true),
                    OldOil = table.Column<int>(type: "integer", nullable: true),
                    OldStatus = table.Column<string>(type: "varchar(50)", nullable: true),
                    NewGas = table.Column<int>(type: "integer", nullable: false),
                    NewOil = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<string>(type: "varchar(50)", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterlyBoilerSalesChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuarterlyBoilerSalesChanges_QuarterlyBoilerSales_QuarterlyB~",
                        column: x => x.QuarterlyBoilerSalesId,
                        principalTable: "QuarterlyBoilerSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuarterlyBoilerSalesFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuarterlyBoilerSalesId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileKey = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuarterlyBoilerSalesFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuarterlyBoilerSalesFiles_QuarterlyBoilerSales_QuarterlyBoi~",
                        column: x => x.QuarterlyBoilerSalesId,
                        principalTable: "QuarterlyBoilerSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualBoilerSalesChanges_AnnualBoilerSalesId",
                table: "AnnualBoilerSalesChanges",
                column: "AnnualBoilerSalesId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualBoilerSalesFiles_AnnualBoilerSalesId",
                table: "AnnualBoilerSalesFiles",
                column: "AnnualBoilerSalesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyBoilerSalesChanges_QuarterlyBoilerSalesId",
                table: "QuarterlyBoilerSalesChanges",
                column: "QuarterlyBoilerSalesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuarterlyBoilerSalesFiles_QuarterlyBoilerSalesId",
                table: "QuarterlyBoilerSalesFiles",
                column: "QuarterlyBoilerSalesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualBoilerSalesChanges");

            migrationBuilder.DropTable(
                name: "AnnualBoilerSalesFiles");

            migrationBuilder.DropTable(
                name: "QuarterlyBoilerSalesChanges");

            migrationBuilder.DropTable(
                name: "QuarterlyBoilerSalesFiles");

            migrationBuilder.DropTable(
                name: "AnnualBoilerSales");

            migrationBuilder.DropTable(
                name: "QuarterlyBoilerSales");
        }
    }
}
