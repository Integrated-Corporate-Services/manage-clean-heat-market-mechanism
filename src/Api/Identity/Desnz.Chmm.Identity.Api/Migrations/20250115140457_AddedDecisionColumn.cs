using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedDecisionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationApprovalFiles");

            migrationBuilder.DropTable(
                name: "OrganisationComments");

            migrationBuilder.CreateTable(
                name: "OrganisationDecisionComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Decision = table.Column<string>(type: "text", nullable: false),
                    ChmmUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationDecisionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationDecisionComments_ChmmUsers_ChmmUserId",
                        column: x => x.ChmmUserId,
                        principalTable: "ChmmUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationDecisionComments_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationDecisionFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileKey = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(100)", nullable: false),
                    Decision = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationDecisionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationDecisionFiles_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationDecisionComments_ChmmUserId",
                table: "OrganisationDecisionComments",
                column: "ChmmUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationDecisionComments_OrganisationId",
                table: "OrganisationDecisionComments",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationDecisionFiles_OrganisationId",
                table: "OrganisationDecisionFiles",
                column: "OrganisationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationDecisionComments");

            migrationBuilder.DropTable(
                name: "OrganisationDecisionFiles");

            migrationBuilder.CreateTable(
                name: "OrganisationApprovalFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    FileKey = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationApprovalFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationApprovalFiles_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChmmUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganisationComments_ChmmUsers_ChmmUserId",
                        column: x => x.ChmmUserId,
                        principalTable: "ChmmUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationComments_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationApprovalFiles_OrganisationId",
                table: "OrganisationApprovalFiles",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationComments_ChmmUserId",
                table: "OrganisationComments",
                column: "ChmmUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationComments_OrganisationId",
                table: "OrganisationComments",
                column: "OrganisationId");
        }
    }
}
