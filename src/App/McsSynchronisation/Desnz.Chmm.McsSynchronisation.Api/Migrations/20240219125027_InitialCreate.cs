using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.McsSynchronisation.Api.Migrations
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
                name: "AirTypeTechnologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirTypeTechnologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlternativeSystemFuelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativeSystemFuelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlternativeSystemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativeSystemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeatPumpProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "varchar(100)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    ManufacturerId = table.Column<int>(type: "integer", nullable: false),
                    ManufacturerName = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatPumpProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstallationAges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallationAges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstallationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TechnologyTypeIds = table.Column<int[]>(type: "integer[]", nullable: true),
                    IsNewBuildIds = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewBuildOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewBuildOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RenewableSystemDesigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenewableSystemDesigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechnologyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeatPumpInstallations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstallationRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    AirTypeTechnologyId = table.Column<int>(type: "integer", nullable: false),
                    AlternativeHeatingFuelId = table.Column<int>(type: "integer", nullable: false),
                    AlternativeHeatingSystemId = table.Column<int>(type: "integer", nullable: false),
                    CommissioningDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    MidId = table.Column<int>(type: "integer", nullable: false),
                    IsNewBuildId = table.Column<int>(type: "integer", nullable: false),
                    IsHybrid = table.Column<bool>(type: "boolean", nullable: false),
                    RenewableSystemDesignId = table.Column<int>(type: "integer", nullable: false),
                    TechnologyTypeId = table.Column<int>(type: "integer", nullable: false),
                    TotalCapacity = table.Column<decimal>(type: "decimal", nullable: false),
                    IsAlternativeHeatingSystemPresent = table.Column<bool>(type: "boolean", nullable: false),
                    AlternativeHeatingAgeId = table.Column<int>(type: "integer", nullable: false),
                    Mpan = table.Column<string>(type: "varchar(100)", nullable: false),
                    CertificatesCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatPumpInstallations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeatPumpInstallations_InstallationRequests_InstallationRequ~",
                        column: x => x.InstallationRequestId,
                        principalTable: "InstallationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeatPumpInstallationProducts",
                columns: table => new
                {
                    InstallationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatPumpInstallationProducts", x => new { x.InstallationId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_HeatPumpInstallationProducts_HeatPumpInstallations_Installa~",
                        column: x => x.InstallationId,
                        principalTable: "HeatPumpInstallations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeatPumpInstallationProducts_HeatPumpProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "HeatPumpProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeatPumpInstallationProducts_ProductId",
                table: "HeatPumpInstallationProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_HeatPumpInstallations_InstallationRequestId",
                table: "HeatPumpInstallations",
                column: "InstallationRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirTypeTechnologies");

            migrationBuilder.DropTable(
                name: "AlternativeSystemFuelTypes");

            migrationBuilder.DropTable(
                name: "AlternativeSystemTypes");

            migrationBuilder.DropTable(
                name: "HeatPumpInstallationProducts");

            migrationBuilder.DropTable(
                name: "InstallationAges");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "NewBuildOptions");

            migrationBuilder.DropTable(
                name: "RenewableSystemDesigns");

            migrationBuilder.DropTable(
                name: "TechnologyTypes");

            migrationBuilder.DropTable(
                name: "HeatPumpInstallations");

            migrationBuilder.DropTable(
                name: "HeatPumpProducts");

            migrationBuilder.DropTable(
                name: "InstallationRequests");
        }
    }
}
