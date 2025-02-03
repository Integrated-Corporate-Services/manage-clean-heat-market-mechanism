using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
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
                name: "ChmmRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChmmRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChmmUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false),
                    Status = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChmmUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChmmRoleChmmUser",
                columns: table => new
                {
                    ChmmRolesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChmmUsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChmmRoleChmmUser", x => new { x.ChmmRolesId, x.ChmmUsersId });
                    table.ForeignKey(
                        name: "FK_ChmmRoleChmmUser_ChmmRoles_ChmmRolesId",
                        column: x => x.ChmmRolesId,
                        principalTable: "ChmmRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChmmRoleChmmUser_ChmmUsers_ChmmUsersId",
                        column: x => x.ChmmUsersId,
                        principalTable: "ChmmUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChmmRoleChmmUser_ChmmUsersId",
                table: "ChmmRoleChmmUser",
                column: "ChmmUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChmmRoleChmmUser");

            migrationBuilder.DropTable(
                name: "ChmmRoles");

            migrationBuilder.DropTable(
                name: "ChmmUsers");
        }
    }
}
