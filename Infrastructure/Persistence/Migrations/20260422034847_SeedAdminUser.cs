using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 3, 48, 46, 127, DateTimeKind.Utc).AddTicks(412));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 3, 48, 46, 127, DateTimeKind.Utc).AddTicks(418));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BlockedAt", "BlockedReason", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "Email", "IsDeleted", "PasswordHash", "Status", "UpdatedBy", "UpdatedDate" },
                values: new object[] { 1, null, null, "seed", new DateTime(2026, 4, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "admin@gmail.com", false, "$2a$11$iV9hWKbffZV/dVcS/MVViu.l/LBM7Jc95XBVSESd0pD91Yc5bSH.a", 1, null, null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 3, 45, 58, 991, DateTimeKind.Utc).AddTicks(1371));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 3, 45, 58, 991, DateTimeKind.Utc).AddTicks(1376));
        }
    }
}
