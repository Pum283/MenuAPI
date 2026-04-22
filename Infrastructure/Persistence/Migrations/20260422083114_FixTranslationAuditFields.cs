using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTranslationAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MenuItemTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "MenuItemTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "MenuCategoryTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "MenuCategoryTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CafeTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CafeTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 8, 31, 13, 697, DateTimeKind.Utc).AddTicks(6158));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 8, 31, 13, 697, DateTimeKind.Utc).AddTicks(6186));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MenuItemTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MenuItemTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MenuCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MenuCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CafeTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CafeTranslations");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 8, 15, 31, 512, DateTimeKind.Utc).AddTicks(4585));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 8, 15, 31, 512, DateTimeKind.Utc).AddTicks(4615));
        }
    }
}
