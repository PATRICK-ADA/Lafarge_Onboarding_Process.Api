using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lafarge_Onboarding.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOnboardingDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BodyContent",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyContentFileType",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyFilePath",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHeading",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentSubHeading",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFilePath",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileType",
                table: "OnboardingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 12, 59, 844, DateTimeKind.Utc).AddTicks(4835));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 12, 59, 844, DateTimeKind.Utc).AddTicks(4844));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 12, 59, 844, DateTimeKind.Utc).AddTicks(4851));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 12, 59, 844, DateTimeKind.Utc).AddTicks(4858));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyContent",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "BodyContentFileType",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "BodyFilePath",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "ContentHeading",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "ContentSubHeading",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "ImageFilePath",
                table: "OnboardingDocuments");

            migrationBuilder.DropColumn(
                name: "ImageFileType",
                table: "OnboardingDocuments");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "OnboardingDocuments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(823));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(833));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(842));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(850));
        }
    }
}
