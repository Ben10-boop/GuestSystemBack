using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuestSystemBack.Migrations
{
    /// <inheritdoc />
    public partial class initialv01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuper = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtraDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisitableEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitableEmployees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitPurpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisiteeId = table.Column<int>(type: "int", nullable: false),
                    WasSubmitByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    SubmitterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSubmissions_Admins_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Admins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormSubmissions_VisitableEmployees_VisiteeId",
                        column: x => x.VisiteeId,
                        principalTable: "VisitableEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormSubmissionId = table.Column<int>(type: "int", nullable: false),
                    ExtraDocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormDocuments_ExtraDocuments_ExtraDocumentId",
                        column: x => x.ExtraDocumentId,
                        principalTable: "ExtraDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormDocuments_FormSubmissions_FormSubmissionId",
                        column: x => x.FormSubmissionId,
                        principalTable: "FormSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormDocuments_ExtraDocumentId",
                table: "FormDocuments",
                column: "ExtraDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocuments_FormSubmissionId",
                table: "FormDocuments",
                column: "FormSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_SubmitterId",
                table: "FormSubmissions",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_VisiteeId",
                table: "FormSubmissions",
                column: "VisiteeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormDocuments");

            migrationBuilder.DropTable(
                name: "ExtraDocuments");

            migrationBuilder.DropTable(
                name: "FormSubmissions");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "VisitableEmployees");
        }
    }
}
