using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuestSystemBack.Migrations
{
    /// <inheritdoc />
    public partial class globalupdatev05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormDocuments_ExtraDocuments_ExtraDocumentId",
                table: "FormDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_FormDocuments_FormSubmissions_FormSubmissionId",
                table: "FormDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_FormSubmissions_Admins_SubmitterId",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_SubmitterId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "SubmitterId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "IsSuper",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "FormSubmissionId",
                table: "FormDocuments",
                newName: "FormId");

            migrationBuilder.RenameColumn(
                name: "ExtraDocumentId",
                table: "FormDocuments",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_FormDocuments_FormSubmissionId",
                table: "FormDocuments",
                newName: "IX_FormDocuments_FormId");

            migrationBuilder.RenameIndex(
                name: "IX_FormDocuments_ExtraDocumentId",
                table: "FormDocuments",
                newName: "IX_FormDocuments_DocumentId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "VisitableEmployees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Signature",
                table: "FormSubmissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ExtraDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FormDocuments_ExtraDocuments_DocumentId",
                table: "FormDocuments",
                column: "DocumentId",
                principalTable: "ExtraDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormDocuments_FormSubmissions_FormId",
                table: "FormDocuments",
                column: "FormId",
                principalTable: "FormSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormDocuments_ExtraDocuments_DocumentId",
                table: "FormDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_FormDocuments_FormSubmissions_FormId",
                table: "FormDocuments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VisitableEmployees");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExtraDocuments");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "FormId",
                table: "FormDocuments",
                newName: "FormSubmissionId");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "FormDocuments",
                newName: "ExtraDocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_FormDocuments_FormId",
                table: "FormDocuments",
                newName: "IX_FormDocuments_FormSubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_FormDocuments_DocumentId",
                table: "FormDocuments",
                newName: "IX_FormDocuments_ExtraDocumentId");

            migrationBuilder.AlterColumn<string>(
                name: "Signature",
                table: "FormSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmitterId",
                table: "FormSubmissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuper",
                table: "Admins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_SubmitterId",
                table: "FormSubmissions",
                column: "SubmitterId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormDocuments_ExtraDocuments_ExtraDocumentId",
                table: "FormDocuments",
                column: "ExtraDocumentId",
                principalTable: "ExtraDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormDocuments_FormSubmissions_FormSubmissionId",
                table: "FormDocuments",
                column: "FormSubmissionId",
                principalTable: "FormSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormSubmissions_Admins_SubmitterId",
                table: "FormSubmissions",
                column: "SubmitterId",
                principalTable: "Admins",
                principalColumn: "Id");
        }
    }
}
