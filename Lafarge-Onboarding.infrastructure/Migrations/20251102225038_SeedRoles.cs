using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lafarge_Onboarding.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(823), "Local hire role", "LOCAL_HIRE", "LOCAL_HIRE" },
                    { "2", null, new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(833), "Expat role", "EXPAT", "EXPAT" },
                    { "3", null, new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(842), "Visitor role", "VISITOR", "VISITOR" },
                    { "4", null, new DateTime(2025, 11, 2, 22, 50, 37, 989, DateTimeKind.Utc).AddTicks(850), "HR Admin role", "HR_ADMIN", "HR_ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");
        }
    }
}
