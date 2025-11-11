using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Lafarge_Onboarding.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalHireInfoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingStatus",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "AspNetUsers",
                newName: "ActiveStatus");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "AspNetUsers",
                newName: "StaffProfilePicture");

            migrationBuilder.CreateTable(
                name: "LocalHireInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WhoWeAre = table.Column<string>(type: "text", nullable: false),
                    FootprintSummary = table.Column<string>(type: "text", nullable: false),
                    Plants = table.Column<string>(type: "text", nullable: false),
                    ReadyMix = table.Column<string>(type: "text", nullable: false),
                    Depots = table.Column<string>(type: "text", nullable: false),
                    CultureSummary = table.Column<string>(type: "text", nullable: false),
                    Pillars = table.Column<string>(type: "text", nullable: false),
                    Innovation = table.Column<string>(type: "text", nullable: false),
                    HuaxinSpirit = table.Column<string>(type: "text", nullable: false),
                    RespectfulWorkplaces = table.Column<string>(type: "text", nullable: false),
                    Introduction = table.Column<string>(type: "text", nullable: false),
                    CountryFacts = table.Column<string>(type: "text", nullable: false),
                    InterestingFacts = table.Column<string>(type: "text", nullable: false),
                    Holidays = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalHireInfos", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 10, 23, 43, 45, 402, DateTimeKind.Utc).AddTicks(5113));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 10, 23, 43, 45, 402, DateTimeKind.Utc).AddTicks(5124));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 10, 23, 43, 45, 402, DateTimeKind.Utc).AddTicks(5132));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 10, 23, 43, 45, 402, DateTimeKind.Utc).AddTicks(5140));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalHireInfos");

            migrationBuilder.RenameColumn(
                name: "StaffProfilePicture",
                table: "AspNetUsers",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "ActiveStatus",
                table: "AspNetUsers",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "OnboardingStatus",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 30, 33, 412, DateTimeKind.Utc).AddTicks(401));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 30, 33, 412, DateTimeKind.Utc).AddTicks(408));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 30, 33, 412, DateTimeKind.Utc).AddTicks(414));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 4, 11, 30, 33, 412, DateTimeKind.Utc).AddTicks(420));
        }
    }
}
