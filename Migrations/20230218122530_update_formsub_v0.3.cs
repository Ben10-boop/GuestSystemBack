using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GuestSystemBack.Migrations
{
    /// <inheritdoc />
    public partial class updateformsubv03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionTime",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "WasSubmitByAdmin",
                table: "FormSubmissions");

            migrationBuilder.RenameColumn(
                name: "VisitEndTime",
                table: "FormSubmissions",
                newName: "EntranceTime");

            migrationBuilder.RenameColumn(
                name: "GuestName",
                table: "FormSubmissions",
                newName: "WifiAccessStatus");

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureTime",
                table: "FormSubmissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "FormSubmissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FormSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureTime",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "FormSubmissions");

            migrationBuilder.RenameColumn(
                name: "WifiAccessStatus",
                table: "FormSubmissions",
                newName: "GuestName");

            migrationBuilder.RenameColumn(
                name: "EntranceTime",
                table: "FormSubmissions",
                newName: "VisitEndTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionTime",
                table: "FormSubmissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "WasSubmitByAdmin",
                table: "FormSubmissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
