using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CafeTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CafeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CafeTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CafeTranslations_Cafes_CafeId",
                        column: x => x.CafeId,
                        principalTable: "Cafes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuCategoryTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    MenuCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategoryTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuCategoryTranslations_MenuCategories_MenuCategoryId",
                        column: x => x.MenuCategoryId,
                        principalTable: "MenuCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItemTranslations_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            // Backfill current single-language columns into VI translations (Language = 1 => Vi)
            migrationBuilder.Sql(@"
INSERT INTO CafeTranslations (CafeId, Language, Name, CreatedDate)
SELECT c.Id, 1, c.Name, SYSUTCDATETIME()
FROM Cafes c
WHERE NOT EXISTS (
    SELECT 1 FROM CafeTranslations ct WHERE ct.CafeId = c.Id AND ct.Language = 1
);");

            migrationBuilder.Sql(@"
INSERT INTO MenuCategoryTranslations (MenuCategoryId, Language, Name, Description, CreatedDate)
SELECT mc.Id, 1, mc.Name, mc.Description, SYSUTCDATETIME()
FROM MenuCategories mc
WHERE NOT EXISTS (
    SELECT 1 FROM MenuCategoryTranslations mct WHERE mct.MenuCategoryId = mc.Id AND mct.Language = 1
);");

            migrationBuilder.Sql(@"
INSERT INTO MenuItemTranslations (MenuItemId, Language, Name, Description, CreatedDate)
SELECT mi.Id, 1, mi.Name, mi.Description, SYSUTCDATETIME()
FROM MenuItems mi
WHERE NOT EXISTS (
    SELECT 1 FROM MenuItemTranslations mit WHERE mit.MenuItemId = mi.Id AND mit.Language = 1
);");

            migrationBuilder.CreateIndex(
                name: "IX_CafeTranslations_CafeId_Language",
                table: "CafeTranslations",
                columns: new[] { "CafeId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategoryTranslations_MenuCategoryId_Language",
                table: "MenuCategoryTranslations",
                columns: new[] { "MenuCategoryId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemTranslations_MenuItemId_Language",
                table: "MenuItemTranslations",
                columns: new[] { "MenuItemId", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CafeTranslations");

            migrationBuilder.DropTable(
                name: "MenuCategoryTranslations");

            migrationBuilder.DropTable(
                name: "MenuItemTranslations");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 4, 54, 25, 730, DateTimeKind.Utc).AddTicks(2498));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 4, 54, 25, 730, DateTimeKind.Utc).AddTicks(2506));
        }
    }
}
